const PAGE_SIZE = 6;

const state = {
    selectedCategories: [],
    searchQuery: '',
    currentPage: 1
};

const presetConfigs = {
    all: {
        query: '',
        categories: [],
        summary: 'Показываем весь каталог без дополнительных ограничений.'
    },
    everyday: {
        query: '',
        categories: ['everyday'],
        summary: 'Подборка для обычных дней: понятные блюда, к которым легко возвращаться.'
    },
    teaBreak: {
        query: '',
        categories: ['tea-break'],
        summary: 'Здесь собраны рецепты для спокойного завтрака и домашнего чаепития.'
    },
    meatless: {
        query: '',
        categories: ['meatless'],
        summary: 'Рецепты без мясного акцента, где основную работу делают овощи, крупы и сыр.'
    },
    quickDinner: {
        query: '',
        categories: ['everyday', 'meatless'],
        summary: 'Небольшая подборка для быстрого ужина без сложных сценариев и лишних продуктов.'
    }
};

document.addEventListener('DOMContentLoaded', async () => {
    await loadFilters();
    setupEventListeners();
    updateCatalogSummary(presetConfigs.all.summary);
    await loadRecipes();
});

async function loadFilters() {
    try {
        const response = await fetch('/api/recipe/filters');
        const result = await response.json();
        if (!result.success) {
            return;
        }

        renderCategories(result.categories);
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
                activatePreset(null);
                updateCatalogSummary('Поиск работает поверх текущих фильтров и показывает более точечную выдачу.');
                loadRecipes();
            }, 250);
        } else {
            hideSuggestions();
            state.searchQuery = '';
            state.currentPage = 1;
            loadRecipes();
        }
    });

    document.getElementById('applyBtn').addEventListener('click', () => {
        updateSelectedFilters();
        state.currentPage = 1;
        activatePreset(null);
        updateCatalogSummary('Показываем результаты по вручную собранным фильтрам.');
        loadRecipes();
    });

    document.getElementById('resetBtn').addEventListener('click', () => {
        resetFilters();
        activatePreset('all');
        updateCatalogSummary(presetConfigs.all.summary);
        loadRecipes();
    });

    document.querySelectorAll('.preset-chip').forEach(chip => {
        chip.addEventListener('click', () => applyPreset(chip.dataset.preset || 'all'));
    });
}

function resetFilters() {
    document.querySelectorAll('.category-filter').forEach(el => {
        el.checked = false;
    });

    document.getElementById('searchInput').value = '';
    state.searchQuery = '';
    state.selectedCategories = [];
    state.currentPage = 1;
    hideSuggestions();
}

function applyPreset(presetName) {
    const preset = presetConfigs[presetName] || presetConfigs.all;

    state.searchQuery = preset.query;
    state.selectedCategories = [...preset.categories];
    state.currentPage = 1;

    document.getElementById('searchInput').value = preset.query;

    document.querySelectorAll('.category-filter').forEach(input => {
        input.checked = state.selectedCategories.includes(input.value);
    });

    activatePreset(presetName);
    updateCatalogSummary(preset.summary);
    hideSuggestions();
    loadRecipes();
}

function activatePreset(presetName) {
    document.querySelectorAll('.preset-chip').forEach(chip => {
        chip.classList.toggle('is-active', presetName !== null && chip.dataset.preset === presetName);
    });
}

function updateCatalogSummary(text) {
    const summary = document.getElementById('catalogSummary');
    if (summary) {
        summary.textContent = text;
    }
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

    suggestions.forEach(suggestion => {
        const item = document.createElement('li');
        item.className = 'suggestion-item';
        item.textContent = suggestion.name + (suggestion.type === 'category' ? ' (категория)' : '');
        item.addEventListener('click', () => {
            document.getElementById('searchInput').value = suggestion.name;
            state.searchQuery = suggestion.name;
            state.currentPage = 1;
            hideSuggestions();
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
}

async function loadRecipes() {
    try {
        updateSelectedFilters();

        const params = new URLSearchParams();
        if (state.searchQuery) params.append('query', state.searchQuery);
        state.selectedCategories.forEach(cat => params.append('categories', cat));
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
    }
}

function getRecipeUrl(recipe) {
    if (!recipe.slug) return null;
    return `/recipe/${encodeURIComponent(recipe.slug)}`;
}

function renderRecipes(recipes) {
    const container = document.getElementById('cards');
    container.innerHTML = '';

    if (recipes.length === 0) {
        container.innerHTML = '<article class="catalog-empty"><p class="card-kicker">Пустой результат</p><h3>По этому сценарию пока ничего не найдено.</h3><p>Попробуйте снять часть фильтров или выбрать другой быстрый режим сверху.</p></article>';
        return;
    }

    recipes.forEach(recipe => {
        const recipeUrl = getRecipeUrl(recipe);
        const card = document.createElement('article');
        card.className = 'recipe-card-wrapper';
        if (recipeUrl) {
            card.style.cursor = 'pointer';
        }

        const preview = document.createElement('div');
        preview.className = 'recipe-preview';

        const info = document.createElement('div');
        info.className = 'recipe-info';

        const image = document.createElement('img');
        image.src = recipe.imageFileName && recipe.imageFileName.startsWith('https://')
            ? recipe.imageFileName
            : `/images/${recipe.imageFileName}`;
        image.alt = recipe.name;

        const kicker = document.createElement('p');
        kicker.className = 'card-kicker';
        kicker.textContent = recipe.category || 'Рецепт';

        const title = document.createElement('h4');
        title.textContent = recipe.name;

        const description = document.createElement('p');
        description.className = 'recipe-description';
        description.textContent = recipe.description;

        const infoTitle = document.createElement('h4');
        infoTitle.textContent = recipe.name;

        const badges = document.createElement('div');
        badges.className = 'catalog-badges';
        badges.innerHTML = `
            <span>${recipe.category || 'Подборка не указана'}</span>
            <span>${recipe.cookingTime} мин</span>
        `;

        const meta = document.createElement('p');
        meta.className = 'meta';
        meta.innerHTML = `
            <strong>Подборка:</strong> ${recipe.category || 'Не указана'}<br>
            <strong>Время:</strong> ${recipe.cookingTime} мин
        `;

        const infoDescription = document.createElement('p');
        infoDescription.className = 'recipe-description';
        infoDescription.textContent = recipe.description;

        preview.appendChild(image);
        preview.appendChild(kicker);
        preview.appendChild(title);
        preview.appendChild(description);

        info.appendChild(infoTitle);
        info.appendChild(badges);
        info.appendChild(meta);
        info.appendChild(infoDescription);

        card.appendChild(preview);
        card.appendChild(info);

        if (recipeUrl) {
            card.addEventListener('click', () => {
                window.location.href = recipeUrl;
            });
        }

        container.appendChild(card);
    });
}

function updateRecipesCount(count) {
    document.getElementById('recipesCount').textContent = `Найдено рецептов: ${count}`;
}

function renderPagination(currentPage, totalPages) {
    const container = document.getElementById('pagination');
    container.innerHTML = '';

    if (totalPages <= 1) return;

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
