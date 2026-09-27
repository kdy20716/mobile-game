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
        private const int Port = 8089;
        private static bool _isRunning = false;

        private static volatile bool _isPlaying = false;
        private static volatile bool _isCompiling = false;
        private static volatile string _lastBuildStatus = "idle";

        static UnityMcpBridge()
        {
            StartServer();
            EditorApplication.quitting += StopServer;
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

            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://localhost:{Port}/");
                _listener.Start();
                _isRunning = true;

                _listenerThread = new Thread(ListenLoop);
                _listenerThread.IsBackground = true;
                _listenerThread.Start();

                Debug.Log($"<color=cyan><b>[Unity MCP Bridge]</b> 유니티 MCP 서버가 포트 {Port}에서 실행 중입니다.</color>");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Unity MCP Bridge] 시작 중 오류 (권한 또는 포트 점유): {ex.Message}");
            }
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
                    responseString = "{\"status\":\"ok\",\"service\":\"UnityMcpBridge\",\"version\":\"1.1\"}";
                }
            }
            catch (Exception ex)
            {
                responseString = $"{{\"error\":\"{ex.Message}\"}}";
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            res.ContentLength64 = buffer.Length;
            using (var output = res.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
#endif
