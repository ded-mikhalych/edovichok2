const PAGE_SIZE = 4;
const VIEW_HISTORY_CLIENT_KEY = 'recipe-history-client-key';
const VIEW_HISTORY_LIMIT = 6;
const COMMENT_PREVIEW_LIMIT = 2;
const DEFAULT_EXTERNAL_IMAGE = 'https://placehold.co/600x400/F4EEE2/F4EEE2.png';

const state = {
    selectedCategories: [],
    selectedIngredients: [],
    searchQuery: '',
    currentPage: 1
};

let recipesAbortController = null;
let activeCommentCard = null;

document.addEventListener('DOMContentLoaded', async () => {
    await renderViewHistory();
    await loadFilters();
    setupEventListeners();
    await loadRecipes();
});

function setupEventListeners() {
    const searchInput = document.getElementById('searchInput');
    const searchBtn = document.getElementById('searchBtn');
    const clearHistoryBtn = document.getElementById('clearHistoryBtn');
    const applyBtn = document.getElementById('applyBtn');
    const resetBtn = document.getElementById('resetBtn');
    let searchTimeout;

    searchInput?.addEventListener('input', event => {
        const query = event.target.value.trim();
        clearTimeout(searchTimeout);

        if (query.length > 0) {
            searchTimeout = setTimeout(() => {
                getSearchSuggestions(query);
            }, 250);
            return;
        }

        hideSuggestions();
    });

    searchInput?.addEventListener('keydown', event => {
        if (event.key !== 'Enter') {
            return;
        }

        event.preventDefault();
        performSearch();
    });

    searchBtn?.addEventListener('click', () => {
        performSearch();
    });

    applyBtn?.addEventListener('click', () => {
        updateSelectedFilters();
        state.currentPage = 1;
        loadRecipes();
    });

    resetBtn?.addEventListener('click', () => {
        resetFilters();
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

function performSearch() {
    const searchInput = document.getElementById('searchInput');
    state.searchQuery = searchInput?.value.trim() || '';
    state.currentPage = 1;
    hideSuggestions();
    loadRecipes();
}

async function loadFilters() {
    try {
        const response = await fetch('/api/recipe/filters');
        const result = await response.json();
        if (!result.success) {
            return;
        }

        renderFilterOptions('categoriesContainer', result.categories || [], item => item.name, item => item.displayName, 'category-filter');
        renderFilterOptions('ingredientsContainer', result.ingredients || [], item => item.name, item => item.name, 'ingredient-filter');
    } catch (error) {
        console.error('Error loading filters:', error);
    }
}

function renderFilterOptions(containerId, items, valueSelector, textSelector, inputClass) {
    const container = document.getElementById(containerId);
    if (!container) {
        return;
    }

    container.innerHTML = '';

    items.forEach(item => {
        const label = document.createElement('label');
        label.className = 'filter-option';

        const input = document.createElement('input');
        input.type = 'checkbox';
        input.value = valueSelector(item);
        input.className = inputClass;

        const text = document.createElement('span');
        text.textContent = textSelector(item);

        label.appendChild(input);
        label.appendChild(text);
        container.appendChild(label);
    });
}

function resetFilters() {
    document.querySelectorAll('.category-filter, .ingredient-filter').forEach(el => {
        el.checked = false;
    });

    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.value = '';
    }

    state.searchQuery = '';
    state.selectedCategories = [];
    state.selectedIngredients = [];
    state.currentPage = 1;
    hideSuggestions();
}

async function getSearchSuggestions(query) {
    try {
        const response = await fetch(`/api/recipe/suggestions?query=${encodeURIComponent(query)}`);
        const result = await response.json();

        if (result.success && result.data.length > 0) {
            renderSuggestions(result.data);
            return;
        }

        hideSuggestions();
    } catch (error) {
        console.error('Error fetching suggestions:', error);
    }
}

function renderSuggestions(suggestions) {
    const list = document.getElementById('suggestionsList');
    if (!list) {
        return;
    }

    list.innerHTML = '';

    suggestions.forEach(suggestion => {
        const item = document.createElement('li');
        item.className = 'suggestion-item';
        item.textContent = suggestion.name;
        item.addEventListener('click', () => {
            const searchInput = document.getElementById('searchInput');
            if (searchInput) {
                searchInput.value = suggestion.name;
            }

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
    updateSelectedFilters();

    if (recipesAbortController) {
        recipesAbortController.abort();
    }

    recipesAbortController = new AbortController();

    try {
        const params = new URLSearchParams();
        if (state.searchQuery) params.append('query', state.searchQuery);
        state.selectedCategories.forEach(value => params.append('categories', value));
        state.selectedIngredients.forEach(value => params.append('ingredients', value));
        params.append('currentPage', String(state.currentPage));
        params.append('pageSize', String(PAGE_SIZE));

        const response = await fetch(`/api/recipe/search?${params.toString()}`, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            signal: recipesAbortController.signal
        });

        const result = await response.json();
        if (!result.success) {
            throw new Error(result.message || 'Search request failed.');
        }

        state.currentPage = Number.isInteger(result.page) ? result.page : state.currentPage;
        renderRecipes(Array.isArray(result.data) ? result.data : []);
        updateRecipesCount(result.count || 0);
        renderPagination(result.page || 1, result.totalPages || 0);
    } catch (error) {
        if (error.name === 'AbortError') {
            return;
        }

        console.error('Error loading recipes:', error);
        const cards = document.getElementById('cards');
        const pagination = document.getElementById('pagination');

        if (cards) {
            cards.innerHTML = '<article class="catalog-empty"><p class="card-kicker">Ошибка</p><h3>Не удалось загрузить рецепты.</h3><p>Попробуйте обновить страницу немного позже.</p></article>';
        }

        if (pagination) {
            pagination.innerHTML = '';
        }
    }
}

function getRecipeUrl(recipe) {
    return recipe.slug ? `/recipe/${encodeURIComponent(recipe.slug)}` : null;
}

function getRecipeImageSrc(imageFileName) {
    if (!imageFileName) {
        return DEFAULT_EXTERNAL_IMAGE;
    }

    return imageFileName.startsWith('data:image/') || imageFileName.startsWith('https://')
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
    return { 'X-Client-Key': getClientKey() };
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
        const response = await fetch(`/api/recipe/history?limit=${VIEW_HISTORY_LIMIT}`, {
            headers: getClientHeaders()
        });
        const result = await response.json();
        const history = response.ok && result.success && Array.isArray(result.data) ? result.data : [];

        section.hidden = history.length === 0;
        container.innerHTML = '';

        history.forEach(recipe => {
            const card = document.createElement('a');
            card.className = 'catalog-history-card';
            card.href = getRecipeUrl(recipe) ?? '/in-development';

            const image = document.createElement('img');
            image.src = getRecipeImageSrc(recipe.imageFileName);
            image.alt = recipe.name;

            const body = document.createElement('div');
            body.className = 'catalog-history-body';

            const kicker = document.createElement('p');
            kicker.className = 'card-kicker';
            kicker.textContent = recipe.category || 'Рецепт';

            const title = document.createElement('h4');
            title.textContent = recipe.name;

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

function formatCommentTime(value) {
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) {
        return '';
    }

    return new Intl.DateTimeFormat('ru-RU', {
        day: '2-digit',
        month: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    }).format(date);
}

function createCommentItem(comment) {
    const item = document.createElement('article');
    item.className = 'catalog-comment-item';

    const meta = document.createElement('div');
    meta.className = 'catalog-comment-meta';
    meta.textContent = formatCommentTime(comment.createdAt);

    const body = document.createElement('p');
    body.className = 'catalog-comment-text';
    body.textContent = comment.body;

    item.appendChild(meta);
    item.appendChild(body);
    return item;
}

function renderRecipeComments(list, comments) {
    list.innerHTML = '';

    if (!comments.length) {
        const empty = document.createElement('p');
        empty.className = 'catalog-comment-empty';
        empty.textContent = 'Пока нет комментариев. Будьте первым.';
        list.appendChild(empty);
        return;
    }

    comments.forEach(comment => {
        list.appendChild(createCommentItem(comment));
    });
}

async function loadRecipeComments(recipeId) {
    const response = await fetch(`/api/recipe/${recipeId}/comments?limit=${COMMENT_PREVIEW_LIMIT}`, {
        headers: getClientHeaders()
    });

    const result = await response.json();
    if (!response.ok || !result.success) {
        throw new Error(result.message || 'Не удалось загрузить комментарии.');
    }

    return Array.isArray(result.data) ? result.data : [];
}

async function openCommentPopup(shell, recipe) {
    if (activeCommentCard && activeCommentCard !== shell) {
        closeCommentPopup(activeCommentCard);
    }

    activeCommentCard = shell;
    shell.classList.add('is-commenting');

    const popup = shell.querySelector('.catalog-comment-popup');
    const list = shell.querySelector('.catalog-comment-list');
    const status = shell.querySelector('.catalog-comment-status');
    const textarea = shell.querySelector('.catalog-comment-input');

    if (!popup || !list || !status || !textarea) {
        return;
    }

    popup.hidden = false;
    status.textContent = 'Загрузка комментариев...';

    try {
        const comments = await loadRecipeComments(recipe.id);
        renderRecipeComments(list, comments);
        status.textContent = '';
    } catch (error) {
        console.error('Error loading recipe comments:', error);
        status.textContent = 'Не удалось загрузить комментарии.';
    }

    textarea.focus();
}

function closeCommentPopup(shell) {
    const popup = shell?.querySelector('.catalog-comment-popup');
    if (popup) {
        popup.hidden = true;
    }

    shell?.classList.remove('is-commenting');

    if (activeCommentCard === shell) {
        activeCommentCard = null;
    }
}

function renderRecipes(recipes) {
    const container = document.getElementById('cards');
    if (!container) {
        return;
    }

    activeCommentCard = null;
    container.innerHTML = '';

    if (recipes.length === 0) {
        container.innerHTML = '<article class="catalog-empty"><p class="card-kicker">Пустой результат</p><h3>По этому сочетанию фильтров пока ничего не найдено.</h3><p>Попробуйте снять часть ограничений или выбрать другой ингредиент.</p></article>';
        return;
    }

    recipes.forEach(recipe => {
        const shell = document.createElement('article');
        shell.className = 'catalog-card-shell';

        const card = document.createElement('a');
        card.className = 'editorial-card catalog-editorial-card';
        card.href = getRecipeUrl(recipe) ?? '/in-development';
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

        const actions = document.createElement('div');
        actions.className = 'catalog-card-actions';

        const commentButton = document.createElement('button');
        commentButton.type = 'button';
        commentButton.className = 'button button-secondary catalog-comment-button';
        commentButton.textContent = 'Комментарий';
        commentButton.addEventListener('click', async event => {
            event.preventDefault();
            event.stopPropagation();

            if (shell.classList.contains('is-commenting')) {
                closeCommentPopup(shell);
                return;
            }

            await openCommentPopup(shell, recipe);
        });

        const popup = document.createElement('div');
        popup.className = 'catalog-comment-popup';
        popup.hidden = true;
        popup.addEventListener('click', event => {
            event.stopPropagation();
        });

        const popupTitle = document.createElement('p');
        popupTitle.className = 'catalog-comment-title';
        popupTitle.textContent = 'Короткий комментарий';

        const commentList = document.createElement('div');
        commentList.className = 'catalog-comment-list';

        const commentStatus = document.createElement('p');
        commentStatus.className = 'catalog-comment-status';

        const form = document.createElement('form');
        form.className = 'catalog-comment-form';

        const textarea = document.createElement('textarea');
        textarea.className = 'catalog-comment-input';
        textarea.name = 'comment';
        textarea.rows = 3;
        textarea.maxLength = 160;
        textarea.placeholder = 'Напишите короткий комментарий...';

        const submitButton = document.createElement('button');
        submitButton.type = 'submit';
        submitButton.className = 'button button-primary catalog-comment-submit';
        submitButton.textContent = 'Отправить';

        form.addEventListener('submit', async event => {
            event.preventDefault();
            event.stopPropagation();

            const bodyValue = textarea.value.trim();
            commentStatus.textContent = '';

            if (bodyValue.length < 3) {
                commentStatus.textContent = 'Введите хотя бы 3 символа.';
                return;
            }

            submitButton.disabled = true;

            try {
                const response = await fetch(`/api/recipe/${recipe.id}/comments`, {
                    method: 'POST',
                    headers: {
                        ...getClientHeaders(),
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ body: bodyValue })
                });

                const result = await response.json();
                if (!response.ok || !result.success) {
                    throw new Error(result.message || 'Не удалось сохранить комментарий.');
                }

                textarea.value = '';
                const updatedComments = await loadRecipeComments(recipe.id);
                renderRecipeComments(commentList, updatedComments);
                commentStatus.textContent = 'Комментарий отправлен.';
            } catch (error) {
                console.error('Error saving recipe comment:', error);
                commentStatus.textContent = error.message || 'Не удалось сохранить комментарий.';
            } finally {
                submitButton.disabled = false;
            }
        });

        form.appendChild(textarea);
        form.appendChild(submitButton);
        popup.appendChild(popupTitle);
        popup.appendChild(commentList);
        popup.appendChild(commentStatus);
        popup.appendChild(form);
        actions.appendChild(commentButton);

        shell.appendChild(card);
        shell.appendChild(actions);
        shell.appendChild(popup);
        container.appendChild(shell);
    });
}

function updateRecipesCount(count) {
    const counter = document.getElementById('recipesCount');
    if (counter) {
        counter.textContent = `Найдено рецептов: ${count}`;
    }
}

function renderPagination(currentPage, totalPages) {
    const container = document.getElementById('pagination');
    if (!container) {
        return;
    }

    container.innerHTML = '';
    if (totalPages <= 1) {
        return;
    }

    container.appendChild(createPaginationButton('<', currentPage - 1, currentPage <= 1, false));

    for (let page = 1; page <= totalPages; page++) {
        container.appendChild(createPaginationButton(String(page), page, false, page === currentPage));
    }

    container.appendChild(createPaginationButton('>', currentPage + 1, currentPage >= totalPages, false));
}

function createPaginationButton(label, page, disabled, isActive) {
    const button = document.createElement('button');
    button.type = 'button';
    button.className = 'page-btn' + (isActive ? ' active' : '');
    button.textContent = label;
    button.disabled = disabled;

    button.addEventListener('click', event => {
        event.preventDefault();
        if (disabled || page < 1 || page === state.currentPage) {
            return;
        }

        state.currentPage = page;
        loadRecipes();
    });

    return button;
}

document.addEventListener('click', event => {
    if (!event.target.closest('.search-wrapper')) {
        hideSuggestions();
    }

    if (activeCommentCard && !activeCommentCard.contains(event.target)) {
        closeCommentPopup(activeCommentCard);
    }
});
