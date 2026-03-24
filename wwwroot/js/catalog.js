const PAGE_SIZE = 6;

const state = {
    selectedCategories: [],
    selectedIngredients: [],
    searchQuery: '',
    currentPage: 1
};

const previewCache = new Map();
let hoverCard = null;
let hoverHideTimeout = null;

const presetConfigs = {
    all: {
        query: '',
        categories: [],
        ingredients: [],
        summary: 'Показываем весь каталог без дополнительных ограничений.'
    },
    firstCourse: {
        query: '',
        categories: ['first-course'],
        ingredients: [],
        summary: 'Подборка первых блюд: супы, бульоны и другие варианты для основательного начала обеда.'
    },
    secondCourse: {
        query: '',
        categories: ['second-course'],
        ingredients: [],
        summary: 'Здесь собраны вторые блюда, которые можно поставить в центр обычного домашнего ужина.'
    },
    pastry: {
        query: '',
        categories: ['pastry'],
        ingredients: [],
        summary: 'Раздел с выпечкой: пироги, запеканки и другие рецепты для духовки.'
    },
    drinks: {
        query: '',
        categories: ['drinks'],
        ingredients: [],
        summary: 'Здесь будут напитки: домашние, простые и рассчитанные на понятные ингредиенты.'
    }
};

document.addEventListener('DOMContentLoaded', async () => {
    await loadFilters();
    ensureHoverCard();
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
                activatePreset(null);
                updateCatalogSummary('Поиск работает поверх выбранного типа блюда и ингредиентов.');
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
        updateCatalogSummary('Показываем результаты по выбранному типу блюда и отмеченным ингредиентам.');
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

function applyPreset(presetName) {
    const preset = presetConfigs[presetName] || presetConfigs.all;

    state.searchQuery = preset.query;
    state.selectedCategories = [...preset.categories];
    state.selectedIngredients = [...preset.ingredients];
    state.currentPage = 1;

    document.getElementById('searchInput').value = preset.query;

    document.querySelectorAll('.category-filter').forEach(input => {
        input.checked = state.selectedCategories.includes(input.value);
    });

    document.querySelectorAll('.ingredient-filter').forEach(input => {
        input.checked = state.selectedIngredients.includes(input.value);
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
        container.innerHTML = '<article class="catalog-empty"><p class="card-kicker">Пустой результат</p><h3>По этому сочетанию фильтров пока ничего не найдено.</h3><p>Попробуйте снять часть ограничений или выбрать другой ингредиент.</p></article>';
        return;
    }

    recipes.forEach(recipe => {
        const recipeUrl = getRecipeUrl(recipe);
        const card = document.createElement('article');
        card.className = 'recipe-card-wrapper';
        card.dataset.recipeId = String(recipe.id);
        if (recipeUrl) {
            card.style.cursor = 'pointer';
        }

        const compactCard = document.createElement('div');
        compactCard.className = 'recipe-compact-card';

        const image = document.createElement('img');
        image.src = recipe.imageFileName && recipe.imageFileName.startsWith('https://')
            ? recipe.imageFileName
            : `/images/${recipe.imageFileName}`;
        image.alt = recipe.name;

        const kicker = document.createElement('p');
        kicker.className = 'recipe-compact-kicker';
        kicker.textContent = recipe.category || 'Рецепт';

        const title = document.createElement('h4');
        title.className = 'recipe-compact-title';
        title.textContent = recipe.name;

        compactCard.appendChild(image);
        compactCard.appendChild(kicker);
        compactCard.appendChild(title);

        card.appendChild(compactCard);

        card.addEventListener('mouseenter', () => {
            showRecipeHover(card, recipe.id);
        });

        card.addEventListener('mouseleave', scheduleHideHoverCard);

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

function ensureHoverCard() {
    if (hoverCard) return;

    hoverCard = document.createElement('div');
    hoverCard.className = 'catalog-hover-card';
    hoverCard.innerHTML = `
        <h4></h4>
        <div class="catalog-hover-badges"></div>
        <p class="catalog-hover-meta"></p>
        <p class="catalog-hover-description"></p>
    `;

    hoverCard.addEventListener('mouseenter', () => {
        if (hoverHideTimeout) {
            clearTimeout(hoverHideTimeout);
            hoverHideTimeout = null;
        }
    });

    hoverCard.addEventListener('mouseleave', scheduleHideHoverCard);

    document.body.appendChild(hoverCard);
}

async function showRecipeHover(card, recipeId) {
    if (hoverHideTimeout) {
        clearTimeout(hoverHideTimeout);
        hoverHideTimeout = null;
    }

    positionHoverCard(card);
    hoverCard.classList.add('is-visible');

    if (previewCache.has(recipeId)) {
        applyHoverContent(previewCache.get(recipeId));
        return;
    }

    try {
        const response = await fetch(`/api/recipe/${recipeId}/preview`);
        const result = await response.json();

        if (!response.ok || !result.success) {
            throw new Error(result.message || 'Не удалось загрузить описание');
        }

        previewCache.set(recipeId, result.data);
        applyHoverContent(result.data);
    } catch (error) {
        console.error('Error loading preview:', error);
    }
}

function applyHoverContent(data) {
    if (!hoverCard) return;

    hoverCard.querySelector('h4').textContent = data.name;

    const badges = hoverCard.querySelector('.catalog-hover-badges');
    badges.innerHTML = `
        <span class="catalog-hover-badge">${data.category}</span>
        <span class="catalog-hover-badge">${data.cookingTime} мин</span>
    `;

    hoverCard.querySelector('.catalog-hover-meta').innerHTML = `
        <strong>Тип блюда:</strong> ${data.category}<br>
        <strong>Время:</strong> ${data.cookingTime} мин
    `;

    hoverCard.querySelector('.catalog-hover-description').textContent = data.description;
}

function positionHoverCard(card) {
    if (!hoverCard) return;

    const rect = card.getBoundingClientRect();
    const panelWidth = Math.min(360, window.innerWidth - 32);
    let left = rect.right + 18;
    let top = rect.top;

    if (left + panelWidth > window.innerWidth - 16) {
        left = rect.left - panelWidth - 18;
    }

    if (left < 16) {
        left = Math.max(16, rect.left);
        top = rect.bottom + 12;
    }

    const maxTop = window.innerHeight - 300;
    hoverCard.style.left = `${Math.max(16, left)}px`;
    hoverCard.style.top = `${Math.max(16, Math.min(top, maxTop))}px`;
}

function scheduleHideHoverCard() {
    hoverHideTimeout = setTimeout(() => {
        if (hoverCard) {
            hoverCard.classList.remove('is-visible');
        }
    }, 120);
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

window.addEventListener('scroll', () => {
    if (hoverCard) {
        hoverCard.classList.remove('is-visible');
    }
}, { passive: true });

window.addEventListener('resize', () => {
    if (hoverCard) {
        hoverCard.classList.remove('is-visible');
    }
});
