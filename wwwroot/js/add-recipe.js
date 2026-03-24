document.addEventListener('DOMContentLoaded', () => {
    const ingredientsDiv = document.getElementById('ingredients');
    const stepsDiv = document.getElementById('steps');
    const addIngredientBtn = document.getElementById('addIngredient');
    const addStepBtn = document.getElementById('addStep');
    const recipeForm = document.getElementById('recipeForm');
    const imageInput = document.getElementById('recipeImage');

    let stepCount = 0;

    addIngredientBtn.addEventListener('click', () => {
        const div = document.createElement('div');
        div.classList.add('ingredient');
        div.innerHTML = `
            <div class="input-row">
                <input type="text" name="ingredients[]" placeholder="Например: 200 г муки" required>
                <button type="button" onclick="this.closest('.ingredient').remove()">Удалить</button>
            </div>
        `;
        ingredientsDiv.appendChild(div);
    });

    addStepBtn.addEventListener('click', () => {
        stepCount++;
        const div = document.createElement('div');
        div.classList.add('step');
        div.setAttribute('draggable', 'true');
        div.innerHTML = `
            <span class="drag-handle" title="Перетащите, чтобы изменить порядок">⋰</span>
            <h3>Шаг ${stepCount}</h3>
            <textarea name="steps[]" placeholder="Описание шага" required></textarea>
            <button type="button" onclick="this.parentElement.remove()">Удалить шаг</button>
        `;
        stepsDiv.appendChild(div);
        makeStepDraggable(div);
        updateStepNumbers();
    });

    if (imageInput) {
        const imagePreview = document.createElement('img');
        imagePreview.classList.add('preview');
        imagePreview.alt = 'Предпросмотр блюда';
        imagePreview.style.display = 'none';
        imageInput.insertAdjacentElement('afterend', imagePreview);

        imageInput.addEventListener('change', () => handleFileSelect(imageInput, imagePreview));
    }

    recipeForm.addEventListener('submit', async event => {
        if (event.defaultPrevented) return;
        event.preventDefault();

        const difficultyMap = { easy: 1, medium: 2, hard: 3 };
        const titleInput = recipeForm.querySelector('input[name="title"]');
        const descriptionInput = recipeForm.querySelector('textarea[name="description"]');
        const authorInput = recipeForm.querySelector('input[name="author"]');
        const cuisineInput = recipeForm.querySelector('select[name="cuisine"]');
        const difficultyInput = recipeForm.querySelector('select[name="difficulty"]');
        const mainImageInput = recipeForm.querySelector('#recipeImage');

        const ingredients = Array.from(recipeForm.querySelectorAll('input[name="ingredients[]"]'))
            .map(input => input.value.trim())
            .filter(Boolean);

        const steps = Array.from(recipeForm.querySelectorAll('textarea[name="steps[]"]'))
            .map(textarea => textarea.value.trim())
            .filter(Boolean);

        const formData = new FormData();
        formData.append('Title', titleInput?.value?.trim() || '');
        formData.append('Description', descriptionInput?.value?.trim() || '');
        formData.append('Author', authorInput?.value?.trim() || '');
        formData.append('Cuisine', cuisineInput?.value?.trim() || '');
        formData.append('Difficulty', String(difficultyMap[difficultyInput?.value] || 0));
        formData.append('CookingTime', '30');

        ingredients.forEach(item => formData.append('Ingredients', item));
        steps.forEach(item => formData.append('Steps', item));

        if (mainImageInput?.files?.[0]) {
            formData.append('MainImage', mainImageInput.files[0]);
        }

        const submitBtn = recipeForm.querySelector('input[type="submit"]');
        const originalText = submitBtn?.value || 'Сохранить';

        if (submitBtn) {
            submitBtn.disabled = true;
            submitBtn.value = 'Сохраняем...';
        }

        try {
            const response = await fetch('/api/recipe', {
                method: 'POST',
                body: formData
            });

            const result = await response.json();
            if (!response.ok || !result.success) {
                throw new Error(result.message || 'Не удалось сохранить рецепт');
            }

            alert('Рецепт успешно сохранён');
            if (result.data?.slug) {
                window.location.href = `/recipe/${encodeURIComponent(result.data.slug)}`;
                return;
            }

            window.location.href = '/catalog';
        } catch (error) {
            alert(error.message || 'Ошибка при сохранении рецепта');
        } finally {
            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.value = originalText;
            }
        }
    });

    function handleFileSelect(input, previewImg) {
        const file = input.files[0];
        if (!file) return;

        window.ImageEditor.open(file, (dataUrl, blob) => {
            if (dataUrl && blob) {
                previewImg.src = dataUrl;
                previewImg.style.display = 'block';

                const newFile = new File([blob], file.name, { type: blob.type });
                const dataTransfer = new DataTransfer();
                dataTransfer.items.add(newFile);
                input.files = dataTransfer.files;
            } else {
                input.value = '';
                previewImg.src = '';
                previewImg.style.display = 'none';
            }
        });
    }

    function makeStepDraggable(stepEl) {
        stepEl.addEventListener('dragstart', event => {
            stepEl.classList.add('dragging');
            event.dataTransfer.effectAllowed = 'move';
        });

        stepEl.addEventListener('dragend', () => {
            stepEl.classList.remove('dragging');
            document.querySelectorAll('.step.drag-over').forEach(el => el.classList.remove('drag-over'));
            updateStepNumbers();
        });

        stepEl.addEventListener('dragover', event => {
            event.preventDefault();
            event.dataTransfer.dropEffect = 'move';
            event.currentTarget.classList.add('drag-over');
        });

        stepEl.addEventListener('dragleave', event => {
            event.currentTarget.classList.remove('drag-over');
        });

        stepEl.addEventListener('drop', event => {
            event.preventDefault();
            const dragging = document.querySelector('.step.dragging');
            const droppedOn = event.currentTarget;
            if (!dragging || dragging === droppedOn) return;

            const rect = droppedOn.getBoundingClientRect();
            const offset = event.clientY - rect.top;
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
        steps.forEach((step, index) => {
            const header = step.querySelector('h3');
            if (header) {
                header.textContent = `Шаг ${index + 1}`;
            }
        });
    }
});
