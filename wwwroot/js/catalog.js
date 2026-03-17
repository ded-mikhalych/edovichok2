const PAGE_SIZE = 8;

const state = {
    selectedCategories: [],
    selectedDifficulties: [],
    searchQuery: '',
    currentPage: 1,
    totalPages: 1
};

const RATED_KEY = 'rated_recipe_';

document.addEventListener('DOMContentLoaded', async () => {
    await loadFilters();
    await loadRecipes();
    setupEventListeners();
});

async function loadFilters() {
    try {
        const response = await fetch('/api/recipe/filters');
        const result = await response.json();
        if (result.success) {
            renderCategories(result.categories);
            renderDifficulties(result.difficulties);
        }
    } catch (error) {
        console.error('Error loading filters:', error);
    }
}

function renderCategories(categories) {
    const container = document.getElementById('categoriesContainer');
    container.innerHTML = '';
    categories.forEach(category => {
        const label = document.createElement('p');
        const input = document.createElement('input');
        input.type = 'checkbox';
        input.value = category.name;
        input.className = 'category-filter';
        label.appendChild(input);
        label.appendChild(document.createTextNode(' ' + category.displayName));
        container.appendChild(label);
    });
}

function renderDifficulties(difficulties) {
    const container = document.getElementById('difficultiesContainer');
    container.innerHTML = '';
    difficulties.forEach(diff => {
        const label = document.createElement('p');
        const input = document.createElement('input');
        input.type = 'checkbox';
        input.value = diff.id;
        input.className = 'difficulty-filter';
        label.appendChild(input);
        label.appendChild(document.createTextNode(' ' + diff.displayName));
        container.appendChild(label);
    });
}

function setupEventListeners() {
    const searchInput = document.getElementById('searchInput');
    let searchTimeout;

    searchInput.addEventListener('input', (e) => {
        const query = e.target.value.trim();
        clearTimeout(searchTimeout);
        if (query.length > 0) {
            getSearchSuggestions(query);
            searchTimeout = setTimeout(() => {
                state.searchQuery = query;
                state.currentPage = 1;
                loadRecipes();
            }, 300);
        } else {
            document.getElementById('suggestionsList').style.display = 'none';
            state.searchQuery = '';
            state.currentPage = 1;
            loadRecipes();
        }
    });

    document.getElementById('applyBtn').addEventListener('click', () => {
        updateSelectedFilters();
        state.currentPage = 1;
        loadRecipes();
    });

    document.getElementById('resetBtn').addEventListener('click', () => {
        document.querySelectorAll('.category-filter, .difficulty-filter').forEach(el => el.checked = false);
        document.getElementById('searchInput').value = '';
        state.searchQuery = '';
        state.selectedCategories = [];
        state.selectedDifficulties = [];
        state.currentPage = 1;
        document.getElementById('suggestionsList').style.display = 'none';
        loadRecipes();
    });
}

async function getSearchSuggestions(query) {
    try {
        const response = await fetch(`/api/recipe/suggestions?query=${encodeURIComponent(query)}`);
        const result = await response.json();
        if (result.success && result.data.length > 0) {
            renderSuggestions(result.data);
        } else {
            document.getElementById('suggestionsList').style.display = 'none';
        }
    } catch (error) {
        console.error('Error fetching suggestions:', error);
    }
}

function renderSuggestions(suggestions) {
    const list = document.getElementById('suggestionsList');
    list.innerHTML = '';
    suggestions.forEach(suggestion => {
        const li = document.createElement('li');
        li.style.cssText = 'padding: 8px; cursor: pointer; border-bottom: 1px solid #eee;';
        li.textContent = suggestion.name + (suggestion.type === 'category' ? ' (категория)' : '');
        li.addEventListener('mouseover', () => li.style.backgroundColor = '#f0f0f0');
        li.addEventListener('mouseout', () => li.style.backgroundColor = 'transparent');
        li.addEventListener('click', () => {
            document.getElementById('searchInput').value = suggestion.name;
            state.searchQuery = suggestion.name;
            state.currentPage = 1;
            list.style.display = 'none';
            loadRecipes();
        });
        list.appendChild(li);
    });
    list.style.display = 'block';
}

function updateSelectedFilters() {
    state.selectedCategories = Array.from(document.querySelectorAll('.category-filter:checked')).map(el => el.value);
    state.selectedDifficulties = Array.from(document.querySelectorAll('.difficulty-filter:checked')).map(el => parseInt(el.value));
}

async function loadRecipes() {
    try {
        updateSelectedFilters();
        const params = new URLSearchParams();
        if (state.searchQuery) params.append('query', state.searchQuery);
        state.selectedCategories.forEach(cat => params.append('categories', cat));
        state.selectedDifficulties.forEach(diff => params.append('difficulties', diff));
        params.append('page', state.currentPage);
        params.append('pageSize', PAGE_SIZE);

        const response = await fetch(`/api/recipe/search?${params.toString()}`);
        const result = await response.json();

        if (result.success) {
            state.totalPages = result.totalPages;
            renderRecipes(result.data);
            updateRecipesCount(result.count);
            renderPagination(result.page, result.totalPages);
        }
    } catch (error) {
        console.error('Error loading recipes:', error);
        document.getElementById('cards').innerHTML = '<p style="color: red;">Ошибка при загрузке рецептов</p>';
    }
}

function buildStars(avg, count) {
    const rounded = Math.round(avg);
    let stars = '';
    for (let i = 1; i <= 5; i++) {
        stars += `<span class="${i <= rounded ? 'filled' : ''}" data-val="${i}">★</span>`;
    }
    const countText = count > 0 ? `${avg.toFixed(1)} (${count} оц.)` : 'нет оценок';
    return { stars, countText };
}

function getRecipeUrl(recipe) {
    if (!recipe.slug) return null;
    return `/home/recipe/${encodeURIComponent(recipe.slug)}`;
}

function renderRecipes(recipes) {
    const container = document.getElementById('cards');
    container.innerHTML = '';

    if (recipes.length === 0) {
        container.innerHTML = '<p style="color: #666;">По вашему запросу ничего не найдено</p>';
        return;
    }

    recipes.forEach(recipe => {
        const alreadyRated = localStorage.getItem(RATED_KEY + recipe.id) === '1';
        const recipeUrl = getRecipeUrl(recipe);

        const card = document.createElement('article');
        card.className = 'recipe-card-wrapper';
        card.style.cssText = recipeUrl ? 'cursor:pointer;' : '';

        const preview = document.createElement('div');
        preview.className = 'recipe-preview';

        const info = document.createElement('div');
        info.className = 'recipe-info';

        const img = document.createElement('img');
        img.src = recipe.imageFileName.startsWith('https://') ? recipe.imageFileName : `/images/${recipe.imageFileName}`;
        img.alt = recipe.name;
        img.style.cssText = 'display:block;';

        const title = document.createElement('h4');
        title.textContent = recipe.name;
        title.style.cssText = 'margin:6px 0;';

        const infoTitle = document.createElement('h4');
        infoTitle.textContent = recipe.name;
        infoTitle.style.cssText = 'margin:2px 0 8px;';

        const description = document.createElement('p');
        description.className = 'recipe-description';
        description.textContent = recipe.description;

        const infoDescription = document.createElement('p');
        infoDescription.className = 'recipe-description';
        infoDescription.textContent = recipe.description;

        const meta = document.createElement('p');
        meta.className = 'meta';
        meta.innerHTML = `
            <strong>Кухня:</strong> ${recipe.category}<br>
            <strong>Сложность:</strong> ${getDifficultyText(recipe.difficulty)}<br>
            <strong>Время:</strong> ${recipe.cookingTime} мин
        `;

        const starSpans = starRow.querySelectorAll('span');
        starSpans.forEach((star, idx) => {
            star.addEventListener('mouseenter', () => {
                if (alreadyRated) return;
                starSpans.forEach((s, i) => {
                    s.classList.toggle('hovered', i <= idx);
                    s.classList.remove('filled');
                });
            });
            star.addEventListener('mouseleave', () => {
                if (alreadyRated) return;
                const currentAvg = Math.round(parseFloat(avgLabel.dataset.avg || '0'));
                starSpans.forEach((s, i) => {
                    s.classList.remove('hovered');
                    s.classList.toggle('filled', i < currentAvg);
                });
            });
            star.addEventListener('click', async (e) => {
                e.stopPropagation();
                if (alreadyRated) return;
                const rating = idx + 1;
                try {
                    const resp = await fetch(`/api/recipe/${recipe.id}/rate`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ rating })
                    });
                    const res = await resp.json();
                    if (res.success) {
                        localStorage.setItem(RATED_KEY + recipe.id, '1');
                        avgLabel.dataset.avg = res.averageRating;
                        avgLabel.textContent = `${res.averageRating} (${res.ratingCount} оц.)`;
                        const newRounded = Math.round(res.averageRating);
                        starSpans.forEach((s, i) => {
                            s.classList.remove('hovered');
                            s.classList.toggle('filled', i < newRounded);
                        });
                        ratedLabel.textContent = 'Вы уже оценили';
                    }
                } catch (err) {
                    console.error('Rating error:', err);
                }
            });
        });

        avgLabel.dataset.avg = recipe.averageRating;
        const initialRounded = Math.round(recipe.averageRating);
        starSpans.forEach((s, i) => s.classList.toggle('filled', i < initialRounded));

        preview.appendChild(img);
        preview.appendChild(title);
        preview.appendChild(description);

        info.appendChild(infoTitle);
        info.appendChild(meta);
        info.appendChild(infoDescription);
        info.appendChild(ratingBlock);

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

function getDifficultyText(difficulty) {
    const map = { 1: 'Лёгкое', 2: 'Среднее', 3: 'Сложное' };
    return map[difficulty] || 'Неизвестно';
}

function updateRecipesCount(count) {
    document.getElementById('recipesCount').textContent = `Найдено рецептов: ${count}`;
}

function renderPagination(currentPage, totalPages) {
    const container = document.getElementById('pagination');
    container.innerHTML = '';

    if (totalPages <= 1) return;

    for (let i = 1; i <= totalPages; i++) {
        const btn = document.createElement('button');
        btn.className = 'page-btn' + (i === currentPage ? ' active' : '');
        btn.textContent = i;
        btn.addEventListener('click', () => {
            if (i !== state.currentPage) {
                state.currentPage = i;
                loadRecipes();
            }
        });
        container.appendChild(btn);
    }
}

document.addEventListener('click', (e) => {
    if (!e.target.matches('#searchInput')) {
        document.getElementById('suggestionsList').style.display = 'none';
    }
});
