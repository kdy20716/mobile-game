/**
 * Fal.ai Image Generation Tool for Antigravity & Unity
 * Supports FLUX 1.1 Pro and Recraft V3
 */

const fs = require('fs');
const path = require('path');

// 1. Read FAL_KEY from .env or environment variable
function getFalKey() {
  if (process.env.FAL_KEY) return process.env.FAL_KEY;
  const envPath = path.resolve(process.cwd(), '.env');
  if (fs.existsSync(envPath)) {
    const lines = fs.readFileSync(envPath, 'utf8').split('\n');
    for (const line of lines) {
      const match = line.match(/^\s*FAL_KEY\s*=\s*(.+)\s*$/);
      if (match) return match[1].trim().replace(/^["']|["']$/g, '');
    }
  }
  return null;
}

// 2. Models mapping
const MODELS = {
  'flux-pro': 'fal-ai/flux-pro/v1.1',
  'flux-dev': 'fal-ai/flux/dev',
  'flux-schnell': 'fal-ai/flux/schnell',
  'recraft-v3': 'fal-ai/recraft-v3'
};

async function generateImage(options) {
  const falKey = getFalKey();
  if (!falKey) {
    console.error('ERROR: FAL_KEY not found!');
    console.error('Please set FAL_KEY in your .env file or environment variable.');
    console.error('Get your key from: https://fal.ai/dashboard/keys');
    process.exit(1);
  }

  const modelKey = options.model || 'flux-pro';
  const endpoint = MODELS[modelKey] || modelKey;
  const url = `https://fal.run/${endpoint}`;

  console.log(`[Fal.ai] Calling model: ${endpoint}`);
  console.log(`[Fal.ai] Prompt: "${options.prompt}"`);

  const requestBody = {
    prompt: options.prompt,
    image_size: options.size || 'square_hd', // square_hd, portrait_16_9, landscape_16_9
    enable_safety_checker: false
  };

  if (modelKey === 'recraft-v3') {
    requestBody.style = options.style || 'vector_illustration'; // vector_illustration, 3d_render, realistic_image
  }

  const startTime = Date.now();
  const response = await fetch(url, {
    method: 'POST',
    headers: {
      'Authorization': `Key ${falKey}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(requestBody)
  });

  if (!response.ok) {
    const errText = await response.text();
    console.error(`[Fal.ai] API Error (${response.status}):`, errText);
    process.exit(1);
  }

  const data = await response.json();
  const duration = ((Date.now() - startTime) / 1000).toFixed(1);
  console.log(`[Fal.ai] Generation complete in ${duration}s!`);

  const imageUrl = data.images && data.images[0] ? data.images[0].url : null;
  if (!imageUrl) {
    console.error('[Fal.ai] No image URL found in response:', data);
    process.exit(1);
  }

  if (options.output) {
    const outPath = path.resolve(process.cwd(), options.output);
    const outDir = path.dirname(outPath);
    if (!fs.existsSync(outDir)) fs.mkdirSync(outDir, { recursive: true });

    console.log(`[Fal.ai] Downloading image to: ${outPath}`);
    const imgRes = await fetch(imageUrl);
    const buffer = await imgRes.arrayBuffer();
    fs.writeFileSync(outPath, Buffer.from(buffer));
    console.log(`[Fal.ai] Successfully saved (${(buffer.byteLength / 1024).toFixed(1)} KB)`);
  } else {
    console.log(`[Fal.ai] Result Image URL: ${imageUrl}`);
  }
}

// Parse command line arguments
const args = process.argv.slice(2);
const options = {};
for (let i = 0; i < args.length; i++) {
  if (args[i] === '--prompt' && args[i + 1]) options.prompt = args[++i];
  else if (args[i] === '--model' && args[i + 1]) options.model = args[++i];
  else if (args[i] === '--output' && args[i + 1]) options.output = args[++i];
  else if (args[i] === '--size' && args[i + 1]) options.size = args[++i];
  else if (args[i] === '--style' && args[i + 1]) options.style = args[++i];
}

if (!options.prompt) {
  console.log('Usage: node tools/fal-image-gen.js --prompt "..." [--model flux-pro|recraft-v3] [--output path/to/save.png] [--size square_hd|portrait_16_9]');
  process.exit(0);
}

generateImage(options);
