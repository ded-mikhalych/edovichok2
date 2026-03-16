/* Простая клиентская обработка изображений: crop / rotate / resize
   API: window.ImageEditor.open(file, callback(dataUrl, blob, info))
*/
(function(){
  function createEditor(){
    const overlay = document.createElement('div');
    overlay.className = 'img-editor-overlay';
    overlay.style.display = 'none';

    const panel = document.createElement('div');
    panel.className = 'img-editor-panel';

    const canvas = document.createElement('canvas');
    canvas.className = 'img-editor-canvas';

    const controls = document.createElement('div');
    controls.className = 'img-editor-controls';

    controls.innerHTML = `
      <div class="controls-row">
        <label>Ширина (px): <input type="number" id="imgEditorWidth" value="800" min="100" step="50"></label>
        <button id="imgRotate">Повернуть 90°</button>
        <button id="imgCropToggle">Обрезать</button>
        <button id="imgApply">Применить</button>
        <button id="imgCancel">Отмена</button>
      </div>
    `;

    panel.appendChild(canvas);
    panel.appendChild(controls);
    overlay.appendChild(panel);
    document.body.appendChild(overlay);

    return {overlay,panel,canvas,controls};
  }

  const editor = createEditor();
  const overlay = editor.overlay;
  const canvas = editor.canvas;
  const ctx = canvas.getContext('2d');
  const inputWidth = editor.controls.querySelector('#imgEditorWidth');
  const btnRotate = editor.controls.querySelector('#imgRotate');
  const btnCropToggle = editor.controls.querySelector('#imgCropToggle');
  const btnApply = editor.controls.querySelector('#imgApply');
  const btnCancel = editor.controls.querySelector('#imgCancel');

  let img = new Image();
  let imgDataUrl = null;
  let rotation = 0; // degrees
  let cropping = false;
  let selection = null; // {x,y,w,h} in canvas coords
  let dragging = false;
  let dragStart = null;

  function fitCanvasToContainer(){
    // fit canvas to viewport while preserving image aspect
    const maxW = Math.min(window.innerWidth - 120, img.width);
    const maxH = Math.min(window.innerHeight - 220, img.height);
    // calculate scale to fit
    const ratio = Math.min(maxW / img.width, maxH / img.height, 1);
    canvas.width = Math.round(img.width * ratio);
    canvas.height = Math.round(img.height * ratio);
    drawImage();
  }

  function drawImage(){
    ctx.clearRect(0,0,canvas.width,canvas.height);
    // draw image scaled to canvas
    ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
    // draw selection if cropping
    if (selection) {
      ctx.save();
      ctx.strokeStyle = 'rgba(255,255,255,0.9)';
      ctx.lineWidth = 2;
      ctx.setLineDash([6,4]);
      ctx.strokeRect(selection.x, selection.y, selection.w, selection.h);
      ctx.restore();
      // dim outside selection
      ctx.save();
      ctx.fillStyle = 'rgba(0,0,0,0.25)';
      ctx.beginPath();
      ctx.rect(0,0,canvas.width,canvas.height);
      ctx.rect(selection.x, selection.y, selection.w, selection.h);
      ctx.fill('evenodd');
      ctx.restore();
    }
  }

  function imageToCanvasData(sel, targetWidth, cb){
    // sel in canvas coords; map to original image coords
    const scaleX = img.width / canvas.width;
    const scaleY = img.height / canvas.height;
    const sx = Math.round(sel.x * scaleX);
    const sy = Math.round(sel.y * scaleY);
    const sw = Math.round(sel.w * scaleX);
    const sh = Math.round(sel.h * scaleY);
    const outW = targetWidth || sw;
    const outH = Math.round(sh * (outW / sw));
    const out = document.createElement('canvas');
    out.width = outW;
    out.height = outH;
    const octx = out.getContext('2d');
    octx.drawImage(img, sx, sy, sw, sh, 0, 0, outW, outH);
    out.toBlob((blob) => {
      const url = URL.createObjectURL(blob);
      // create dataURL as well
      const reader = new FileReader();
      reader.onload = () => cb(reader.result, blob, {width:outW,height:outH});
      reader.readAsDataURL(blob);
    }, 'image/jpeg', 0.85);
  }

  function open(file, callback){
    if (!file) return;
    const reader = new FileReader();
    reader.onload = (e) => {
      imgDataUrl = e.target.result;
      img = new Image();
      img.onload = () => {
        rotation = 0; selection = null; cropping = false;
        inputWidth.value = Math.min(800, img.width);
        fitCanvasToContainer();
        overlay.style.display = 'flex';
      };
      img.src = imgDataUrl;
    };
    reader.readAsDataURL(file);

    // pointer handlers for drawing selection
    let isPointerDown = false;
    function onPointerDown(e){
      if (!cropping) return;
      isPointerDown = true;
      const r = canvas.getBoundingClientRect();
      const x = e.clientX - r.left; const y = e.clientY - r.top;
      dragStart = {x,y};
      selection = {x,y,w:0,h:0};
    }
    function onPointerMove(e){
      if (!cropping || !isPointerDown) return;
      const r = canvas.getBoundingClientRect();
      const x = e.clientX - r.left; const y = e.clientY - r.top;
      selection.w = Math.max(10, x - dragStart.x);
      selection.h = Math.max(10, y - dragStart.y);
      selection.x = dragStart.x;
      selection.y = dragStart.y;
      // clamp
      if (selection.x < 0) selection.x = 0;
      if (selection.y < 0) selection.y = 0;
      if (selection.x + selection.w > canvas.width) selection.w = canvas.width - selection.x;
      if (selection.y + selection.h > canvas.height) selection.h = canvas.height - selection.y;
      drawImage();
    }
    function onPointerUp(){
      if (!cropping) return;
      isPointerDown = false; dragStart = null;
    }

    canvas.addEventListener('pointerdown', onPointerDown);
    window.addEventListener('pointermove', onPointerMove);
    window.addEventListener('pointerup', onPointerUp);

    btnRotate.onclick = () => {
      // quick rotate: rotate image by 90deg using offscreen canvas
      const tmp = document.createElement('canvas');
      const tctx = tmp.getContext('2d');
      tmp.width = img.height; tmp.height = img.width;
      tctx.translate(tmp.width/2, tmp.height/2);
      tctx.rotate(Math.PI/2);
      tctx.drawImage(img, -img.width/2, -img.height/2);
      img = new Image();
      img.onload = () => { fitCanvasToContainer(); };
      img.src = tmp.toDataURL('image/jpeg', 0.9);
    };

    btnCropToggle.onclick = () => {
      cropping = !cropping;
      btnCropToggle.textContent = cropping ? 'Обрезка: Вкл' : 'Обрезать';
      if (!cropping) { selection = null; drawImage(); }
    };

    btnCancel.onclick = () => {
      overlay.style.display = 'none';
      cleanup();
      if (callback) callback(null);
    };

    btnApply.onclick = () => {
      // determine selection: if none, take whole canvas
      const sel = selection || {x:0,y:0,w:canvas.width,h:canvas.height};
      const targetW = parseInt(inputWidth.value,10) || sel.w;
      imageToCanvasData(sel, targetW, (dataUrl, blob, info) => {
        overlay.style.display = 'none';
        cleanup();
        if (callback) callback(dataUrl, blob, info);
      });
    };

    function cleanup(){
      // remove event listeners
      canvas.removeEventListener('pointerdown', onPointerDown);
      window.removeEventListener('pointermove', onPointerMove);
      window.removeEventListener('pointerup', onPointerUp);
    }
  }

  // expose API
  window.ImageEditor = { open };

  // CSS styles
  const style = document.createElement('style');
  style.textContent = `
  .img-editor-overlay{position:fixed;inset:0;display:flex;align-items:center;justify-content:center;z-index:11000;background:rgba(255,255,255,0.75)}
  .img-editor-panel{background:#fff;border:1px solid #ddd;padding:12px;border-radius:8px;max-width:calc(100vw - 80px);max-height:calc(100vh - 80px);overflow:auto}
  .img-editor-canvas{display:block;max-width:100%;height:auto;border-radius:6px;background:#000}
  .img-editor-controls{margin-top:8px}
  .img-editor-controls .controls-row{display:flex;gap:8px;align-items:center}
  `;
  document.head.appendChild(style);

})();
