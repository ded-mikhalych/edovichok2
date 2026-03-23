document.addEventListener('DOMContentLoaded', () => {
    const copyButton = document.getElementById('copyRecipeLink');
    const printButton = document.getElementById('printRecipe');
    const statPills = document.querySelectorAll('.stat-pill');

    if (copyButton) {
        copyButton.addEventListener('click', async () => {
            try {
                await navigator.clipboard.writeText(window.location.href);
                const originalText = copyButton.textContent;
                copyButton.textContent = 'Ссылка скопирована';
                setTimeout(() => {
                    copyButton.textContent = originalText;
                }, 1400);
            } catch (error) {
                console.error('Copy failed', error);
            }
        });
    }

    if (printButton) {
        printButton.addEventListener('click', () => {
            window.print();
        });
    }

    statPills.forEach((pill, index) => {
        pill.style.animationDelay = `${index * 70}ms`;
        pill.classList.add('is-visible');
    });
});
