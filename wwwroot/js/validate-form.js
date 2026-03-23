document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('recipeForm');
    if (!form) return;

    const hints = {
        title: 'Обязательно, минимум 3 символа',
        description: 'Обязательно, от 10 до 250 символов',
        author: 'Можно оставить пустым или указать минимум 2 символа',
        cuisine: 'Обязательно выберите тип блюда',
        difficulty: 'Обязательно выберите сложность',
        'ingredients[]': 'Добавьте хотя бы один ингредиент',
        'steps[]': 'Добавьте хотя бы один шаг длиной от 10 символов',
        image: 'JPG/PNG/WebP до 2 МБ'
    };

    function getErrorEl(input) {
        const field = input.closest('.field');
        return field ? field.querySelector('.error-msg') : null;
    }

    function showHint(input, text) {
        const el = getErrorEl(input);
        if (el) {
            el.textContent = text;
            el.style.color = 'var(--muted)';
        }
    }

    function clearHint(input) {
        const el = getErrorEl(input);
        if (el) {
            el.textContent = '';
        }
    }

    function isFieldValid(input) {
        if (input.name === 'title') return input.value.trim().length >= 3;
        if (input.name === 'description') return input.value.trim().length >= 10;
        if (input.name === 'author') return !input.value.trim() || input.value.trim().length >= 2;
        if (input.name === 'cuisine') return ['first-course', 'second-course', 'pastry', 'drinks'].includes(input.value);
        if (input.name === 'difficulty') return ['easy', 'medium', 'hard'].includes(input.value);
        if (input.name === 'ingredients[]') return input.value.trim().length > 0;
        if (input.name === 'steps[]') return input.value.trim().length >= 10;
        if (input.id === 'recipeImage') {
            if (!input.files?.[0]) return true;
            const file = input.files[0];
            return ['image/jpeg', 'image/png', 'image/webp'].includes(file.type) && file.size <= 2 * 1024 * 1024;
        }
        return true;
    }

    function updateHint(input) {
        if (isFieldValid(input)) {
            clearHint(input);
            return;
        }

        const key = input.name || (input.id === 'recipeImage' ? 'image' : '');
        if (hints[key]) {
            showHint(input, hints[key]);
        }
    }

    form.querySelectorAll('input, textarea, select').forEach(input => {
        const key = input.name || (input.id === 'recipeImage' ? 'image' : '');
        if (hints[key]) {
            showHint(input, hints[key]);
        }

        input.addEventListener('input', () => updateHint(input));
        input.addEventListener('change', () => updateHint(input));
    });

    const descEl = form.querySelector('[name="description"]');
    const descCounter = document.getElementById('descCount');
    const maxDescription = 250;

    function autosizeTextarea(el) {
        if (!el) return;
        el.style.height = 'auto';
        el.style.height = `${el.scrollHeight + 2}px`;
    }

    if (descEl) {
        if (!descEl.hasAttribute('maxlength')) {
            descEl.setAttribute('maxlength', maxDescription);
        }

        if (descCounter) {
            descCounter.textContent = String(descEl.value.length);
        }

        autosizeTextarea(descEl);

        descEl.addEventListener('input', () => {
            if (descEl.value.length > maxDescription) {
                descEl.value = descEl.value.slice(0, maxDescription);
            }

            if (descCounter) {
                descCounter.textContent = String(descEl.value.length);
            }

            autosizeTextarea(descEl);
            updateHint(descEl);
        });
    }

    form.addEventListener('submit', event => {
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
            firstInvalid ??= title;
        }

        if (!desc || desc.value.trim().length < 10) {
            errors.push('Описание должно содержать не менее 10 символов');
            firstInvalid ??= desc;
        }

        if (author && author.value.trim().length > 0 && author.value.trim().length < 2) {
            errors.push('Имя автора должно содержать минимум 2 символа или поле нужно оставить пустым');
            firstInvalid ??= author;
        }

        if (!cuisine || !['first-course', 'second-course', 'pastry', 'drinks'].includes(cuisine.value)) {
            errors.push('Выберите тип блюда');
            firstInvalid ??= cuisine;
        }

        const ingredientsFilled = ingredientInputs.map(input => input.value.trim()).filter(Boolean);
        if (ingredientsFilled.length === 0) {
            errors.push('Добавьте хотя бы один ингредиент');
            firstInvalid ??= ingredientInputs[0];
        }

        const stepsFilled = stepTextareas.map(textarea => textarea.value.trim()).filter(Boolean);
        if (stepsFilled.length === 0) {
            errors.push('Добавьте хотя бы один шаг приготовления');
            firstInvalid ??= stepTextareas[0];
        }

        if (!difficulty || !['easy', 'medium', 'hard'].includes(difficulty.value)) {
            errors.push('Выберите корректную сложность');
            firstInvalid ??= difficulty;
        }

        if (imageInput && imageInput.files?.length > 0) {
            const file = imageInput.files[0];
            const allowed = ['image/jpeg', 'image/png', 'image/webp'];
            const maxSize = 2 * 1024 * 1024;

            if (!allowed.includes(file.type)) {
                errors.push('Изображение должно быть в формате JPG, PNG или WebP');
                firstInvalid ??= imageInput;
            } else if (file.size > maxSize) {
                errors.push('Изображение не должно быть больше 2 МБ');
                firstInvalid ??= imageInput;
            }
        }

        if (errors.length > 0) {
            event.preventDefault();
            alert(`Ошибка в форме:\n- ${errors.join('\n- ')}`);
            if (firstInvalid && typeof firstInvalid.focus === 'function') {
                firstInvalid.focus();
            }
        }
    });
});
