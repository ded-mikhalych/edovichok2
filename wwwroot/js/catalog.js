const PAGE_SIZE = 6;
const VIEW_HISTORY_CLIENT_KEY = 'recipe-history-client-key';
const VIEW_HISTORY_LIMIT = 6;
const DEFAULT_EXTERNAL_IMAGE = 'https://placehold.co/600x400/F4EEE2/F4EEE2.png';

const state = {
    selectedCategories: [],
    selectedIngredients: [],
    searchQuery: '',
    currentPage: 1
};

document.addEventListener('DOMContentLoaded', async () => {
    await renderViewHistory();
    await loadFilters();
    setupEventListeners();
    updateCatalogSummary();
    await loadRecipes();
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
    const clearHistoryBtn = document.getElementById('clearHistoryBtn');
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
                loadRecipes();
            }, 250);
        } else {
            hideSuggestions();
            state.searchQuery = '';
            state.currentPage = 1;
            updateCatalogSummary();
            loadRecipes();
        }
    });

    document.getElementById('applyBtn').addEventListener('click', () => {
        updateSelectedFilters();
        state.currentPage = 1;
        updateCatalogSummary();
        loadRecipes();
    });

    document.getElementById('resetBtn').addEventListener('click', () => {
        resetFilters();
        updateCatalogSummary();
        loadRecipes();
    });

    clearHistoryBtn?.addEventListener('click', async () => {
        try {
            const response = await fetch('/api/recipe/history', {
                method: 'DELETE',
                headers: getClientHeaders()
            });
            const result = await response.json();

            if (response.ok && result.success) {
                await renderViewHistory();
            }
        } catch (error) {
            console.error('Error clearing view history:', error);
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
            loadRecipes();
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

async function loadRecipes() {
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
            renderRecipes(result.data);
            updateRecipesCount(result.count);
            renderPagination(result.page, result.totalPages);
        }
    } catch (error) {
        console.error('Error loading recipes:', error);
        document.getElementById('cards').innerHTML = '<article class="catalog-empty"><p class="card-kicker">Ошибка</p><h3>Не удалось загрузить рецепты.</h3><p>Попробуйте обновить страницу немного позже.</p></article>';
        document.getElementById('pagination').innerHTML = '';
    }
}

function getRecipeUrl(recipe) {
    if (!recipe.slug) return null;
    return `/recipe/${encodeURIComponent(recipe.slug)}`;
}

function getRecipeImageSrc(imageFileName) {
    if (!imageFileName) {
        return DEFAULT_EXTERNAL_IMAGE;
    }

    return imageFileName.startsWith('https://')
        ? imageFileName
        : `/images/${imageFileName}`;
}

function getClientKey() {
    const existingKey = localStorage.getItem(VIEW_HISTORY_CLIENT_KEY);
    if (existingKey) {
        return existingKey;
    }

    const generatedKey = self.crypto?.randomUUID?.() || `client-${Date.now()}-${Math.random().toString(16).slice(2)}`;
    localStorage.setItem(VIEW_HISTORY_CLIENT_KEY, generatedKey);
    return generatedKey;
}

function getClientHeaders() {
    return {
        'X-Client-Key': getClientKey()
    };
}

async function saveViewedRecipe(recipe) {
    if (!recipe?.id) {
        return;
    }

    try {
        await fetch(`/api/recipe/${recipe.id}/history`, {
            method: 'POST',
            headers: getClientHeaders()
        });
    } catch (error) {
        console.error('Error saving viewed recipe:', error);
    }
}

async function renderViewHistory() {
    const section = document.getElementById('viewHistorySection');
    const container = document.getElementById('viewHistoryCards');

    if (!section || !container) {
        return;
    }

    try {
        section.style.padding = '28px';
        section.style.background = '#fff';
        section.style.border = '1px solid rgba(93, 58, 27, 0.1)';
        section.style.borderRadius = '28px';
        section.style.boxShadow = '0 18px 40px rgba(84, 50, 23, 0.08)';
        section.style.marginTop = '28px';

        container.style.display = 'flex';
        container.style.flexWrap = 'nowrap';
        container.style.alignItems = 'stretch';
        container.style.gap = '8px';
        container.style.overflowX = 'auto';
        container.style.overflowY = 'hidden';

        const response = await fetch(`/api/recipe/history?limit=${VIEW_HISTORY_LIMIT}`, {
            headers: getClientHeaders()
        });
        const result = await response.json();
        const history = response.ok && result.success && Array.isArray(result.data) ? result.data : [];

        section.hidden = history.length === 0;
        container.innerHTML = '';

        history.forEach(recipe => {
            const item = {
                slug: recipe.slug,
                name: recipe.name,
                imageFileName: recipe.imageFileName,
                category: recipe.category || 'Рецепт'
            };

            const card = document.createElement('a');
            card.className = 'catalog-history-card';
            card.href = getRecipeUrl(item) ?? '/in-development';
            card.style.display = 'block';
            card.style.flex = '0 0 136px';
            card.style.width = '136px';
            card.style.minWidth = '136px';
            card.style.maxWidth = '136px';

            const image = document.createElement('img');
            image.src = getRecipeImageSrc(item.imageFileName);
            image.alt = item.name;
            image.style.width = '100%';
            image.style.height = '76px';
            image.style.objectFit = 'cover';
            image.style.display = 'block';

            const body = document.createElement('div');
            body.className = 'catalog-history-body';
            body.style.padding = '7px 8px 8px';

            const kicker = document.createElement('p');
            kicker.className = 'card-kicker';
            kicker.textContent = item.category;
            kicker.style.margin = '0 0 3px';
            kicker.style.fontSize = '0.68rem';
            kicker.style.lineHeight = '1.15';

            const title = document.createElement('h4');
            title.textContent = item.name;
            title.style.margin = '0';
            title.style.fontSize = '0.76rem';
            title.style.lineHeight = '1.22';

            body.appendChild(kicker);
            body.appendChild(title);
            card.appendChild(image);
            card.appendChild(body);
            container.appendChild(card);
        });
    } catch (error) {
        console.error('Error loading view history:', error);
        section.hidden = true;
        container.innerHTML = '';
    }
}

function renderRecipes(recipes) {
    const container = document.getElementById('cards');
    container.innerHTML = '';

    if (recipes.length === 0) {
        container.innerHTML = '<article class="catalog-empty"><p class="card-kicker">Пустой результат</p><h3>По этому сочетанию фильтров пока ничего не найдено.</h3><p>Попробуйте снять часть ограничений или выбрать другой ингредиент.</p></article>';
        return;
    }

    recipes.forEach(recipe => {
        const recipeUrl = getRecipeUrl(recipe);
        const card = document.createElement('a');
        card.className = 'editorial-card catalog-editorial-card';
        card.href = recipeUrl ?? '/in-development';
        card.addEventListener('click', async () => {
            await saveViewedRecipe(recipe);
            await renderViewHistory();
        });

        const image = document.createElement('img');
        image.src = getRecipeImageSrc(recipe.imageFileName);
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

        card.appendChild(image);
        card.appendChild(body);
        container.appendChild(card);
    });
}

function updateRecipesCount(count) {
    document.getElementById('recipesCount').textContent = `Найдено рецептов: ${count}`;
}

function renderPagination(currentPage, totalPages) {
    const container = document.getElementById('pagination');
    container.innerHTML = '';

    if (totalPages <= 1) {
        return;
    }

    for (let page = 1; page <= totalPages; page++) {
        const button = document.createElement('button');
        button.className = 'page-btn' + (page === currentPage ? ' active' : '');
        button.textContent = page;
        button.addEventListener('click', () => {
            if (page !== state.currentPage) {
                state.currentPage = page;
                loadRecipes();
            }
        });
        container.appendChild(button);
    }
}

document.addEventListener('click', event => {
    if (!event.target.matches('#searchInput')) {
        hideSuggestions();
    }
});
