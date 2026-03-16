document.addEventListener("DOMContentLoaded", () => {
  const ingredientsDiv = document.getElementById("ingredients");
  const stepsDiv = document.getElementById("steps");
  const addIngredientBtn = document.getElementById("addIngredient");
  const addStepBtn = document.getElementById("addStep");
  let stepCount = 0;

  // Добавление ингредиента
  addIngredientBtn.addEventListener("click", () => {
    const div = document.createElement("div");
    div.classList.add("ingredient");
    div.innerHTML = `
      <div class="input-row">
        <input type="text" name="ingredients[]" placeholder="Например: 200 г муки" required>
        <button type="button" onclick="this.closest('.ingredient').remove()">Удалить</button>
      </div>
    `;
    ingredientsDiv.appendChild(div);
  });

  // Добавление шага с картинкой
  addStepBtn.addEventListener("click", () => {
    stepCount++;
    const div = document.createElement("div");
    div.classList.add("step");
    div.setAttribute('draggable', 'true');
    div.innerHTML = `
      <span class="drag-handle" title="Перетащите, чтобы изменить порядок">☰</span>
      <h3>Шаг ${stepCount}</h3>
      <textarea name="steps[]" placeholder="Описание шага" required></textarea><br>
      <input type="file" name="stepImage${stepCount}" accept="image/*">
      <img class="preview" alt="Предпросмотр шага" style="display:none;">
      <button type="button" onclick="this.parentElement.remove()">Удалить шаг</button>
    `;
    stepsDiv.appendChild(div);
    makeStepDraggable(div);
    updateStepNumbers();

    // Навешиваем обработчик на только что добавленный input file
    const fileInput = div.querySelector('input[type="file"]');
    const previewImg = div.querySelector('img.preview');

    fileInput.addEventListener("change", () => handleFileSelect(fileInput, previewImg));
  });

  // Универсальная функция обработки выбора файла (для шагов и основного фото)
  function handleFileSelect(input, previewImg) {
    const file = input.files[0];
    if (!file) return;

    window.ImageEditor.open(file, function(dataUrl, blob) {
      if (dataUrl && blob) {
        // Пользователь нажал "Применить"
        previewImg.src = dataUrl;
        previewImg.style.display = "block";

        // Заменяем файл в input на отредактированный
        const newFile = new File([blob], file.name, { type: blob.type });
        const dt = new DataTransfer();
        dt.items.add(newFile);
        input.files = dt.files;
      } else {
        // Пользователь отменил — очищаем всё
        input.value = "";
        previewImg.src = "";
        previewImg.style.display = "none";
      }
    });
  }

  // === Основное изображение блюда ===
  const imageInput = document.getElementById("recipeImage");
  if (imageInput) {
    const imagePreview = document.createElement("img");
    imagePreview.classList.add("preview");
    imagePreview.alt = "Предпросмотр блюда";
    imagePreview.style.display = "none";
    imageInput.insertAdjacentElement("afterend", imagePreview);

    imageInput.addEventListener("change", () => handleFileSelect(imageInput, imagePreview));
  }

  // Отправка формы — просто заглушка
  const recipeForm = document.getElementById("recipeForm");
  recipeForm.addEventListener("submit", e => {
    if (e.defaultPrevented) return;
    e.preventDefault();
    alert("Рецепт сохранён.");
  });

  // --- Drag & Drop для шагов ---
  function makeStepDraggable(stepEl) {
    stepEl.addEventListener('dragstart', (ev) => {
      stepEl.classList.add('dragging');
      ev.dataTransfer.effectAllowed = 'move';
    });

    stepEl.addEventListener('dragend', () => {
      stepEl.classList.remove('dragging');
      document.querySelectorAll('.step.drag-over').forEach(el => el.classList.remove('drag-over'));
      updateStepNumbers();
    });

    stepEl.addEventListener('dragover', (ev) => {
      ev.preventDefault();
      ev.dataTransfer.dropEffect = 'move';
      ev.currentTarget.classList.add('drag-over');
    });

    stepEl.addEventListener('dragleave', (ev) => {
      ev.currentTarget.classList.remove('drag-over');
    });

    stepEl.addEventListener('drop', (ev) => {
      ev.preventDefault();
      const dragging = document.querySelector('.step.dragging');
      const droppedOn = ev.currentTarget;
      if (!dragging || dragging === droppedOn) return;

      const rect = droppedOn.getBoundingClientRect();
      const offset = ev.clientY - rect.top;
      const insertBefore = offset < rect.height / 2;

      if (insertBefore) {
        droppedOn.parentElement.insertBefore(dragging, droppedOn);
      } else {
        droppedOn.parentElement.insertBefore(dragging, droppedOn.nextSibling);
      }

      droppedOn.classList.remove('drag-over');
      updateStepNumbers();
    });
  }

  function updateStepNumbers() {
    const steps = Array.from(stepsDiv.querySelectorAll('.step'));
    steps.forEach((s, idx) => {
      const h3 = s.querySelector('h3');
      if (h3) h3.textContent = `Шаг ${idx + 1}`;
    });
  }

  // Инициализация существующих шагов (если редактирование)
  Array.from(stepsDiv.querySelectorAll('.step')).forEach(el => {
    if (!el.querySelector('.drag-handle')) {
      const handle = document.createElement('span');
      handle.className = 'drag-handle';
      handle.textContent = 'Drag';
      handle.title = 'Перетащите, чтобы изменить порядок';
      el.insertBefore(handle, el.firstChild);
    }
    makeStepDraggable(el);

    const fileInput = el.querySelector('input[type="file"]');
    const previewImg = el.querySelector('img.preview');
    if (fileInput && previewImg) {
      fileInput.addEventListener("change", () => handleFileSelect(fileInput, previewImg));
    }
  });
});
