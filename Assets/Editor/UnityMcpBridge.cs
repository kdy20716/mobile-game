#if UNITY_EDITOR
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace MobileRacing.Editor
{
    [InitializeOnLoad]
    public static class UnityMcpBridge
    {
        private static HttpListener _listener;
        private static Thread _listenerThread;
        public static int ActivePort { get; private set; } = 8089;
        private static bool _isRunning = false;
        private static volatile bool _isPlaying = false;
        private static volatile bool _isCompiling = false;
        private static volatile string _lastBuildStatus = "idle";

        static UnityMcpBridge()
        {
            if (UnityEditorInternal.InternalEditorUtility.inBatchMode || Environment.CommandLine.Contains("-batchmode")) return;
            AssemblyReloadEvents.beforeAssemblyReload += StopServer;
            EditorApplication.quitting += StopServer;
            StartServer();
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            _isPlaying = EditorApplication.isPlaying;
            _isCompiling = EditorApplication.isCompiling;
        }

        [MenuItem("Racing Game/MCP Bridge/Restart MCP Server")]
        public static void StartServer()
        {
            StopServer();

            int[] candidatePorts = new int[] { 8089, 8090, 8091, 8092, 8093 };
            foreach (int p in candidatePorts)
            {
                try
                {
                    _listener = new HttpListener();
                    _listener.Prefixes.Add($"http://localhost:{p}/");
                    _listener.Prefixes.Add($"http://127.0.0.1:{p}/");
                    _listener.Start();
                    _isRunning = true;
                    ActivePort = p;

                    _listenerThread = new Thread(ListenLoop);
                    _listenerThread.IsBackground = true;
                    _listenerThread.Start();

                    try { File.WriteAllText("Library/UnityMcpBridgePort.txt", p.ToString()); } catch { }
                    Debug.Log($"<color=cyan><b>[Unity MCP Bridge]</b> 유니티 MCP 서버가 포트 {p}에서 실행 중입니다.</color>");
                    return;
                }
                catch (Exception)
                {
                    if (_listener != null)
                    {
                        try { _listener.Close(); } catch { }
                        _listener = null;
                    }
                }
            }
            Debug.LogWarning("[Unity MCP Bridge] 후보 포트를 모두 바인딩할 수 없습니다.");
        }

        [MenuItem("Racing Game/MCP Bridge/Stop MCP Server")]
        public static void StopServer()
        {
            _isRunning = false;
            if (_listener != null && _listener.IsListening)
            {
                _listener.Stop();
                _listener.Close();
                _listener = null;
            }
        }

        private static void ListenLoop()
        {
            while (_isRunning && _listener != null && _listener.IsListening)
            {
                try
                {
                    var context = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem((state) => HandleRequest(context));
                }
                catch
                {
                    break;
                }
            }
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            var req = context.Request;
            var res = context.Response;

            string responseString = "";
            res.ContentType = "application/json";

            try
            {
                if (req.Url.AbsolutePath == "/status")
                {
                    responseString = $"{{\"isPlaying\":{_isPlaying.ToString().ToLower()},\"isCompiling\":{_isCompiling.ToString().ToLower()},\"lastBuildStatus\":\"{_lastBuildStatus}\"}}";
                }
                else if (req.Url.AbsolutePath == "/recompile")
                {
                    EditorApplication.delayCall += () =>
                    {
                        AssetDatabase.Refresh();
                    };
                    responseString = "{\"status\":\"ok\",\"message\":\"AssetDatabase refreshed\"}";
                }
                else if (req.Url.AbsolutePath == "/generate-blockblast")
                {
                    EditorApplication.delayCall += () =>
                    {
                        BlockBlast.Editor.BlockBlastSceneBuilder.GenerateBlockBlastScene();
                        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
                        AssetDatabase.SaveAssets();
                    };
                    responseString = "{\"status\":\"ok\",\"message\":\"Generating Block Blast Scene\"}";
                }
                else if (req.Url.AbsolutePath == "/check-webgl")
                {
                    bool supported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL);
                    responseString = $"{{\"webglSupported\":{supported.ToString().ToLower()}}}";
                }
                else if (req.Url.AbsolutePath == "/build-webgl")
                {
                    EditorApplication.delayCall += () =>
                    {
                        _lastBuildStatus = "building_webgl";
                        try
                        {
                            BlockBlast.Editor.BlockBlastBuildUtility.BuildWebGL();
                            _lastBuildStatus = "success_webgl";
                        }
                        catch (Exception ex)
                        {
                            _lastBuildStatus = $"failed_webgl: {ex.Message}";
                        }
                    };
                    responseString = "{\"status\":\"ok\",\"message\":\"WebGL Build triggered\"}";
                }
                else if (req.Url.AbsolutePath == "/build-blockblast")
                {
                    EditorApplication.delayCall += () =>
                    {
                        _lastBuildStatus = "building";
                        try
                        {
                            BlockBlast.Editor.BlockBlastBuildUtility.BuildWindows64();
                            _lastBuildStatus = "success";
                        }
                        catch (Exception ex)
                        {
                            _lastBuildStatus = $"failed: {ex.Message}";
                        }
                    };
                    responseString = "{\"status\":\"ok\",\"message\":\"Build triggered\"}";
                }
                else if (req.Url.AbsolutePath == "/build-status")
                {
                    responseString = $"{{\"lastBuildStatus\":\"{_lastBuildStatus}\"}}";
                }
                else
                {
                    responseString = "{\"status\":\"ok\",\"service\":\"UnityMcpBridge\",\"version\":\"1.2\"}";
                }
            }
            catch (Exception ex)
            {
                responseString = $"{{\"error\":\"{ex.Message}\"}}";
            }

            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                res.ContentLength64 = buffer.Length;
                using (var output = res.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
                res.Close();
            }
            catch
            {
                // ignore write errors
            }
        }
    }
}
#endif
