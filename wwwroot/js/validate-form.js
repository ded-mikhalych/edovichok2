document.addEventListener('DOMContentLoaded', () => {
  const form = document.getElementById('recipeForm');
  if (!form) return;

  // === Серые подсказки (видны сразу, исчезают при валидности) ===
  const hints = {
    title: "Обязательно, минимум 3 символа",
    description: "Обязательно, от 10 до 250 символов",
    author: "Минимум 2 символа",
    cuisine: "Русская, азиатская или европейская",
    difficulty: "Обязательно выбрать сложность",
    "ingredients[]": "Добавьте хотя бы один ингредиент",
    "steps[]": "Добавьте хотя бы один шаг (минимум 10 символов)",
    image: "JPG/PNG/WebP до 2 МБ"
  };

  function getErrorEl(input) {
    const field = input.closest('.field');
    return field ? field.querySelector('.error-msg') : null;
  }

  function showHint(input, text) {
    const el = getErrorEl(input);
    if (el) {
      el.textContent = text;
      el.style.color = "var(--muted)";
    }
  }

  function clearHint(input) {
    const el = getErrorEl(input);
    if (el) el.textContent = "";
  }

  function isFieldValid(input) {
    if (input.name === "title") return input.value.trim().length >= 3;
    if (input.name === "description") return input.value.trim().length >= 10;
    if (input.name === "author") return !input.value.trim() || input.value.trim().length >= 2;
    if (input.name === "cuisine") {
      const v = input.value.trim();
      return !v || (v.length >= 3 && /^[\p{L}\s\-]+$/u.test(v));
    }
    if (input.name === "difficulty") return ["easy", "medium", "hard"].includes(input.value);
    if (input.name === "ingredients[]") return input.value.trim().length > 0;
    if (input.name === "steps[]") return input.value.trim().length >= 10;
    if (input.id === "recipeImage") {
      if (!input.files?.[0]) return true;
      const f = input.files[0];
      return ["image/jpeg", "image/png", "image/webp"].includes(f.type) && f.size <= 2 * 1024 * 1024;
    }
    return true;
  }

  function updateHint(input) {
    if (isFieldValid(input)) {
      clearHint(input);
    } else {
      const key = input.name || (input.id === "recipeImage" ? "image" : "");
      if (hints[key]) showHint(input, hints[key]);
    }
  }

  // === Инициализация всех полей ===
  form.querySelectorAll('input, textarea, select').forEach(input => {
    const key = input.name || (input.id === "recipeImage" ? "image" : "");
    if (hints[key]) showHint(input, hints[key]);

    input.addEventListener('input', () => updateHint(input));
    input.addEventListener('change', () => updateHint(input));
  });

  // === Главное изображение — только подсказка (редактор открывает add-recipe.js) ===
  const imageInput = document.getElementById('recipeImage');
  if (imageInput) {
    showHint(imageInput, hints.image);
    // НИЧЕГО НЕ ДЕЛАЕМ с change — это делает add-recipe.js
    // Он уже читает файл и вызывает openImageEditor()
  }

  // === Авторазмер описания + счётчик ===
  const descEl = form.querySelector('[name="description"]');
  const descCounter = document.getElementById('descCount');
  const MAX_DESC = 250;

  function autosizeTextarea(el) {
    if (!el) return;
    el.style.height = 'auto';
    el.style.height = (el.scrollHeight + 2) + 'px';
  }

  if (descEl) {
    if (!descEl.hasAttribute('maxlength')) descEl.setAttribute('maxlength', MAX_DESC);
    if (descCounter) descCounter.textContent = String(descEl.value.length);
    autosizeTextarea(descEl);

    descEl.addEventListener('input', () => {
      const v = descEl.value;
      if (v.length > MAX_DESC) descEl.value = v.slice(0, MAX_DESC);
      if (descCounter) descCounter.textContent = String(descEl.value.length);
      autosizeTextarea(descEl);
      updateHint(descEl);
    });
  }

  // === Твоя оригинальная валидация на submit (без изменений) ===
  form.addEventListener('submit', (e) => {
    const errors = [];
    let firstInvalid = null;

    const title = form.querySelector('[name="title"]');
    const desc = form.querySelector('[name="description"]');
    const author = form.querySelector('[name="author"]');
    const cuisine = form.querySelector('[name="cuisine"]');
    const ingredientInputs = Array.from(form.querySelectorAll('input[name="ingredients[]"]'));
    const stepTextareas = Array.from(form.querySelectorAll('textarea[name="steps[]"]'));
    const difficulty = form.querySelector('[name="difficulty"]');
    const imageInput = form.querySelector('#recipeImage');

    if (!title || title.value.trim().length < 3) {
      errors.push('Название должно содержать не менее 3 символов');
      if (!firstInvalid) firstInvalid = title;
    }

    if (!desc || desc.value.trim().length < 10) {
      errors.push('Описание должно содержать не менее 10 символов');
      if (!firstInvalid) firstInvalid = desc;
    }

    if (author && author.value.trim().length > 0 && author.value.trim().length < 2) {
      errors.push('Имя автора должно содержать как минимум 2 символа или оставьте поле пустым');
      if (!firstInvalid) firstInvalid = author;
    }

    if (cuisine && cuisine.value.trim().length > 0) {
      if (cuisine.value.trim().length < 3) {
        errors.push('Поле "Кухня" должно содержать не менее 3 символов');
        if (!firstInvalid) firstInvalid = cuisine;
      } else if (!/^[\p{L}\s\-]+$/u.test(cuisine.value.trim())) {
        errors.push('Поле "Кухня" должно содержать только буквы, пробелы и дефисы');
        if (!firstInvalid) firstInvalid = cuisine;
      }
    }

    const ingredientsFilled = ingredientInputs.map(i => i.value.trim()).filter(v => v.length > 0);
    if (ingredientsFilled.length === 0) {
      errors.push('Добавьте хотя бы один ингредиент');
      if (!firstInvalid && ingredientInputs.length) firstInvalid = ingredientInputs[0];
    }

    const stepsFilled = stepTextareas.map(t => t.value.trim()).filter(v => v.length > 0);
    if (stepsFilled.length === 0) {
      errors.push('Добавьте хотя бы один шаг приготовления');
      if (!firstInvalid && stepTextareas.length) firstInvalid = stepTextareas[0];
    }

    if (!difficulty || !['easy', 'medium', 'hard'].includes(difficulty.value)) {
      errors.push('Выберите корректную сложность');
      if (!firstInvalid) firstInvalid = difficulty;
    }

    if (imageInput && imageInput.files && imageInput.files.length > 0) {
      const file = imageInput.files[0];
      const allowed = ['image/jpeg', 'image/png', 'image/webp'];
      const maxSize = 2 * 1024 * 1024;
      if (!allowed.includes(file.type)) {
        errors.push('Изображение должно быть в формате JPG/PNG/WebP');
        if (!firstInvalid) firstInvalid = imageInput;
      } else if (file.size > maxSize) {
        errors.push('Изображение не должно быть больше 2 МБ');
        if (!firstInvalid) firstInvalid = imageInput;
      }
    }

    if (errors.length) {
      e.preventDefault();
      alert('Ошибка в форме:\n- ' + errors.join('\n- '));
      if (firstInvalid && typeof firstInvalid.focus === 'function') firstInvalid.focus();
    }
  });
});
