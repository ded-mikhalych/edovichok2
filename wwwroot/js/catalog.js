const PAGE_SIZE = 6;

const state = {
    selectedCategories: [],
    selectedIngredients: [],
    searchQuery: '',
    currentPage: 1,
    totalPages: 1,
    totalCount: 0
};

const previewCache = new Map();

document.addEventListener('DOMContentLoaded', async () => {
    await loadFilters();
    setupEventListeners();
    updateCatalogSummary();
    await loadRecipes({ append: false });
});

async function loadFilters() {
    try {
        const response = await fetch('/api/recipe/filters');
        const result = await response.json();
        if (!result.success) {
            return;
        }

        renderCategories(result.categories || []);
        renderIngredients(result.ingredients || []);
    } catch (error) {
        console.error('Error loading filters:', error);
    }
}

function renderCategories(categories) {
    const container = document.getElementById('categoriesContainer');
    container.innerHTML = '';

    categories.forEach(category => {
        const label = document.createElement('label');
        label.className = 'filter-option';

        const input = document.createElement('input');
        input.type = 'checkbox';
        input.value = category.name;
        input.className = 'category-filter';

        const text = document.createElement('span');
        text.textContent = category.displayName;

        label.appendChild(input);
        label.appendChild(text);
        container.appendChild(label);
    });
}

function renderIngredients(ingredients) {
    const container = document.getElementById('ingredientsContainer');
    container.innerHTML = '';

    ingredients.forEach(ingredient => {
        const label = document.createElement('label');
        label.className = 'filter-option';

        const input = document.createElement('input');
        input.type = 'checkbox';
        input.value = ingredient.name;
        input.className = 'ingredient-filter';

        const text = document.createElement('span');
        text.textContent = ingredient.name;

        label.appendChild(input);
        label.appendChild(text);
        container.appendChild(label);
    });
}

function setupEventListeners() {
    const searchInput = document.getElementById('searchInput');
    let searchTimeout;

    searchInput.addEventListener('input', event => {
        const query = event.target.value.trim();
        clearTimeout(searchTimeout);

        if (query.length > 0) {
            getSearchSuggestions(query);
            searchTimeout = setTimeout(() => {
                state.searchQuery = query;
                state.currentPage = 1;
                updateCatalogSummary();
                loadRecipes({ append: false });
            }, 250);
        } else {
            hideSuggestions();
            state.searchQuery = '';
            state.currentPage = 1;
            updateCatalogSummary();
            loadRecipes({ append: false });
        }
    });

    document.getElementById('applyBtn').addEventListener('click', () => {
        updateSelectedFilters();
        state.currentPage = 1;
        updateCatalogSummary();
        loadRecipes({ append: false });
    });

    document.getElementById('resetBtn').addEventListener('click', () => {
        resetFilters();
        updateCatalogSummary();
        loadRecipes({ append: false });
    });

    document.getElementById('loadMoreBtn').addEventListener('click', () => {
        if (state.currentPage < state.totalPages) {
            state.currentPage += 1;
            loadRecipes({ append: true });
        }
    });

    document.getElementById('closePreviewBtn').addEventListener('click', closePreview);
    document.getElementById('recipePreviewModal').addEventListener('click', event => {
        if (event.target instanceof HTMLElement && event.target.dataset.closePreview === 'true') {
            closePreview();
        }
    });

    document.addEventListener('keydown', event => {
        if (event.key === 'Escape') {
            closePreview();
        }
    });
}

function resetFilters() {
    document.querySelectorAll('.category-filter, .ingredient-filter').forEach(el => {
        el.checked = false;
    });

    document.getElementById('searchInput').value = '';
    state.searchQuery = '';
    state.selectedCategories = [];
    state.selectedIngredients = [];
    state.currentPage = 1;
    hideSuggestions();
}

function updateCatalogSummary() {
    const summary = document.getElementById('catalogSummary');
    if (!summary) {
        return;
    }

    const parts = [];

    if (state.searchQuery) {
        parts.push(`поиск: ${state.searchQuery}`);
    }

    if (state.selectedCategories.length > 0) {
        parts.push(`тип блюда: ${state.selectedCategories.length}`);
    }

    if (state.selectedIngredients.length > 0) {
        parts.push(`ингредиенты: ${state.selectedIngredients.length}`);
    }

    summary.textContent = parts.length > 0
        ? `Активные параметры: ${parts.join(' • ')}.`
        : 'Каталог собирает результаты по текущему поиску и выбранным фильтрам.';
}

async function getSearchSuggestions(query) {
    try {
        const response = await fetch(`/api/recipe/suggestions?query=${encodeURIComponent(query)}`);
        const result = await response.json();

        if (result.success && result.data.length > 0) {
            renderSuggestions(result.data);
        } else {
            hideSuggestions();
        }
    } catch (error) {
        console.error('Error fetching suggestions:', error);
    }
}

function renderSuggestions(suggestions) {
    const list = document.getElementById('suggestionsList');
    list.innerHTML = '';

    const typeLabels = {
        category: 'тип блюда',
        ingredient: 'ингредиент',
        recipe: 'рецепт'
    };

    suggestions.forEach(suggestion => {
        const item = document.createElement('li');
        item.className = 'suggestion-item';
        item.textContent = `${suggestion.name} (${typeLabels[suggestion.type] || 'совпадение'})`;
        item.addEventListener('click', () => {
            document.getElementById('searchInput').value = suggestion.name;
            state.searchQuery = suggestion.name;
            state.currentPage = 1;
            hideSuggestions();
            updateCatalogSummary();
            loadRecipes({ append: false });
        });
        list.appendChild(item);
    });

    list.style.display = 'block';
}

function hideSuggestions() {
    const list = document.getElementById('suggestionsList');
    if (list) {
        list.style.display = 'none';
    }
}

function updateSelectedFilters() {
    state.selectedCategories = Array.from(document.querySelectorAll('.category-filter:checked')).map(el => el.value);
    state.selectedIngredients = Array.from(document.querySelectorAll('.ingredient-filter:checked')).map(el => el.value);
}

async function loadRecipes({ append }) {
    try {
        updateSelectedFilters();

        const params = new URLSearchParams();
        if (state.searchQuery) params.append('query', state.searchQuery);
        state.selectedCategories.forEach(cat => params.append('categories', cat));
        state.selectedIngredients.forEach(ingredient => params.append('ingredients', ingredient));
        params.append('page', state.currentPage);
        params.append('pageSize', PAGE_SIZE);

        const response = await fetch(`/api/recipe/search?${params.toString()}`);
        const result = await response.json();

        if (result.success) {
            state.totalPages = result.totalPages || 1;
            state.totalCount = result.count || 0;

            renderRecipes(result.data, append);
            updateRecipesCount(result.count);
            renderLoadMoreButton();
        }
    } catch (error) {
        console.error('Error loading recipes:', error);
        document.getElementById('cards').innerHTML = '<article class="catalog-empty"><p class="card-kicker">Ошибка</p><h3>Не удалось загрузить рецепты.</h3><p>Попробуйте обновить страницу немного позже.</p></article>';
        renderLoadMoreButton();
    }
}

function getRecipeUrl(recipe) {
    if (!recipe.slug) return null;
    return `/recipe/${encodeURIComponent(recipe.slug)}`;
}

function renderRecipes(recipes, append) {
    const container = document.getElementById('cards');

    if (!append) {
        container.innerHTML = '';
    }

    if (!append && recipes.length === 0) {
        container.innerHTML = '<article class="catalog-empty"><p class="card-kicker">Пустой результат</p><h3>По этому сочетанию фильтров пока ничего не найдено.</h3><p>Попробуйте снять часть ограничений или выбрать другой ингредиент.</p></article>';
        return;
    }

    recipes.forEach(recipe => {
        const recipeUrl = getRecipeUrl(recipe);
        const card = document.createElement('article');
        card.className = 'editorial-card catalog-editorial-card';

        const link = document.createElement('a');
        link.className = 'catalog-card-link';
        link.href = recipeUrl ?? '/in-development';

        const image = document.createElement('img');
        image.src = recipe.imageFileName && recipe.imageFileName.startsWith('https://')
            ? recipe.imageFileName
            : `/images/${recipe.imageFileName}`;
        image.alt = recipe.name;

        const body = document.createElement('div');
        body.className = 'editorial-card-body catalog-editorial-body';

        const kicker = document.createElement('p');
        kicker.className = 'card-kicker';
        kicker.textContent = recipe.category || 'Рецепт';

        const title = document.createElement('h3');
        title.textContent = recipe.name;

        body.appendChild(kicker);
        body.appendChild(title);
        link.appendChild(image);
        link.appendChild(body);

        const previewButton = document.createElement('button');
        previewButton.type = 'button';
        previewButton.className = 'catalog-preview-button';
        previewButton.textContent = 'Быстрый просмотр';
        previewButton.addEventListener('click', () => openPreview(recipe.id, recipe.name));

        card.appendChild(link);
        card.appendChild(previewButton);
        container.appendChild(card);
    });
}

function updateRecipesCount(count) {
    document.getElementById('recipesCount').textContent = `Найдено рецептов: ${count}`;
}

function renderLoadMoreButton() {
    const button = document.getElementById('loadMoreBtn');
    const shouldShow = state.totalCount > 0 && state.currentPage < state.totalPages;

    button.hidden = !shouldShow;
    button.disabled = !shouldShow;
}

async function openPreview(recipeId, recipeName) {
    const modal = document.getElementById('recipePreviewModal');
    const title = document.getElementById('recipePreviewTitle');
    const category = document.getElementById('recipePreviewCategory');
    const time = document.getElementById('recipePreviewTime');
    const description = document.getElementById('recipePreviewDescription');
    const ingredients = document.getElementById('recipePreviewIngredients');
    const link = document.getElementById('recipePreviewLink');

    modal.hidden = false;
    document.body.style.overflow = 'hidden';

    title.textContent = recipeName;
    category.textContent = 'Загрузка...';
    time.textContent = '';
    description.textContent = 'Подгружаем краткое описание рецепта.';
    ingredients.innerHTML = '';
    link.href = '/catalog';

    let preview = previewCache.get(recipeId);

    if (!preview) {
        try {
            const response = await fetch(`/api/recipe/${recipeId}/preview`);
            const result = await response.json();

            if (!result.success) {
                throw new Error('Preview load failed');
            }

            preview = result.data;
            previewCache.set(recipeId, preview);
        } catch (error) {
            category.textContent = 'Рецепт';
            description.textContent = 'Не удалось загрузить быстрый просмотр. Попробуйте открыть полную страницу рецепта.';
            link.textContent = 'Открыть рецепт';
            return;
        }
    }

    category.textContent = preview.category || 'Рецепт';
    time.textContent = preview.cookingTime ? `Время: ${preview.cookingTime} мин` : '';
    description.textContent = preview.description || 'Описание пока не добавлено.';
    link.href = preview.slug ? `/recipe/${encodeURIComponent(preview.slug)}` : '/in-development';
    link.textContent = 'Открыть рецепт';

    ingredients.innerHTML = '';
    (preview.ingredients || []).forEach(item => {
        const li = document.createElement('li');
        li.textContent = item;
        ingredients.appendChild(li);
    });
}

function closePreview() {
    const modal = document.getElementById('recipePreviewModal');
    modal.hidden = true;
    document.body.style.overflow = '';
}

document.addEventListener('click', event => {
    if (!event.target.matches('#searchInput')) {
        hideSuggestions();
    }
});
