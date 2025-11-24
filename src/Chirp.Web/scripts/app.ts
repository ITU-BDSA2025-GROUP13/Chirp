const scrollStorageKey = `chirp-scroll:${window.location.pathname}${window.location.search}`;

const canUseSessionStorage = (() => {
    try {
        if (typeof sessionStorage === "undefined") return false;

        const testKey = "__chirp_scroll_test__";
        sessionStorage.setItem(testKey, "1");
        sessionStorage.removeItem(testKey);
        return true;
    } catch {
        return false;
    }
})();

const canUseLocalStorage = (() => {
    try {
        if (typeof localStorage === "undefined") return false;

        const testKey = "__chirp_theme_test__";
        localStorage.setItem(testKey, "1");
        localStorage.removeItem(testKey);
        return true;
    } catch {
        return false;
    }
})();

type ThemePreference = "light" | "dark";

const themeStorageKey = "chirp-theme";
const defaultLightHref = "/css/colors-light-theme.css";
const defaultDarkHref = "/css/colors-dark-theme.css";

function getStoredTheme(): ThemePreference | null {
    if (!canUseLocalStorage) return null;
    const storedValue = localStorage.getItem(themeStorageKey);
    if (storedValue === "light" || storedValue === "dark") return storedValue;
    return null;
}

function storeTheme(theme: ThemePreference): void {
    if (!canUseLocalStorage) return;
    localStorage.setItem(themeStorageKey, theme);
}

function applyTheme(theme: ThemePreference): void {
    const themeLink = document.getElementById("theme-stylesheet") as HTMLLinkElement | null;
    if (!themeLink) return;

    const lightHref = themeLink.dataset.lightHref ?? defaultLightHref;
    const darkHref = themeLink.dataset.darkHref ?? defaultDarkHref;
    themeLink.href = theme === "dark" ? darkHref : lightHref;
    document.documentElement.setAttribute("data-theme", theme);

    const toggleButton = document.getElementById("themeToggleButton") as HTMLButtonElement | null;
    if (toggleButton) {
        updateToggleButtonAppearance(toggleButton, theme);
    }
}

function updateToggleButtonAppearance(button: HTMLButtonElement, theme: ThemePreference): void {
    const icon = button.querySelector("i");
    if (icon) {
        icon.classList.remove("fa-sun-o", "fa-moon-o");
        icon.classList.add(theme === "dark" ? "fa-sun-o" : "fa-moon-o");
    }

    const ariaLabel = theme === "dark" ? "Switch to light theme" : "Switch to dark theme";
    button.setAttribute("aria-label", ariaLabel);
}

const initialTheme = getStoredTheme() ?? "dark";
applyTheme(initialTheme);

function setupPostEnterBehavior(): void {
    const textarea = document.getElementById('post-text-field') as HTMLTextAreaElement | null;
    const form = document.getElementById('post-form') as HTMLFormElement | null;

    if (!textarea || !form) return;

    // Avoid adding duplicate listeners
    if ((textarea as any)._enterListenerAttached) return;
    (textarea as any)._enterListenerAttached = true;

    textarea.addEventListener("keydown", function (event) {
        if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault();
            submitFormPreservingScroll(form);
        }
    });
}

function persistScrollPosition(): void {
    if (!canUseSessionStorage) return;
    sessionStorage.setItem(scrollStorageKey, window.scrollY.toString());
}

function restoreScrollPosition(): void {
    if (!canUseSessionStorage) return;

    const storedValue = sessionStorage.getItem(scrollStorageKey);
    if (!storedValue) return;

    const scrollY = Number(storedValue);
    sessionStorage.removeItem(scrollStorageKey);
    if (Number.isNaN(scrollY)) return;

    window.requestAnimationFrame(() => {
        window.scrollTo({ top: scrollY, left: window.scrollX });
    });
}

function setupScrollPreservation(): void {
    if (!canUseSessionStorage) return;

    const forms = document.querySelectorAll<HTMLFormElement>('form[data-preserve-scroll]');
    forms.forEach(form => {
        if ((form as any)._scrollPreserverAttached) return;
        form.addEventListener('submit', persistScrollPosition);
        (form as any)._scrollPreserverAttached = true;
    });
}

document.addEventListener('DOMContentLoaded', () => {
    restoreScrollPosition();
    setupScrollPreservation();
    setupThemeToggle(initialTheme);
});

window.addEventListener('pageshow', event => {
    if (event.persisted) {
        restoreScrollPosition();
    }
});

function setupThemeToggle(currentTheme: ThemePreference): void {
    const toggleButton = document.getElementById('themeToggleButton') as HTMLButtonElement | null;
    if (!toggleButton) return;

    if ((toggleButton as any)._themeListenerAttached) return;
    (toggleButton as any)._themeListenerAttached = true;

    updateToggleButtonAppearance(toggleButton, currentTheme);

    toggleButton.addEventListener('click', () => {
        const activeTheme = document.documentElement.getAttribute('data-theme') === 'dark' ? 'dark' : 'light';
        const selectedTheme: ThemePreference = activeTheme === 'dark' ? 'light' : 'dark';
        applyTheme(selectedTheme);
        storeTheme(selectedTheme);
    });
}

/**
 * Toggles the reply UI
 * @param cheepId - the ID to query for
 */
function toggleReply(cheepId: number): void {
    const replyFormWrapper = document.getElementById(`reply-form-wrapper-${cheepId}`);
    const replyTextField = document.getElementById(`reply-textarea-${cheepId}`);
    if (!replyFormWrapper || !replyTextField) return;

    if (replyFormWrapper.style.display === 'none' || replyFormWrapper.style.display === '') {
        replyFormWrapper.style.display = 'block';
        replyTextField.focus();
        setupReplyEnterBehavior(cheepId);
    } else {
        replyFormWrapper.style.display = 'none';
    }

    setupScrollPreservation();
}

/**
 * @param {string} cheepId - the cheepId to query for
 */
function editCheep(cheepId: number): void {
    const cheep = document.getElementById(`cheep-${cheepId}`);
    if (!cheep) return;

    const cheepText = cheep.querySelector<HTMLElement>(`#cheep-text-${cheepId}`);
    const cheepEdit = cheep.querySelector<HTMLElement>(`#cheep-edit-${cheepId}`);
    const cheepEditInput = cheep.querySelector<HTMLTextAreaElement>(`#cheep-edit-input-${cheepId}`);
    if (!cheepText || !cheepEdit || !cheepEditInput) return;

    if (cheepText.style.display === "none") {
        cheepText.style.display = "block";
        cheepEdit.style.display = "none";
    } else {
        cheepText.style.display = "none";
        cheepEdit.style.display = "block";
        cheepEditInput.focus();
        const len = cheepEditInput.value.length;
        cheepEditInput.setSelectionRange(len, len);
    }
}

/**
 * @param cheepId - the cheepId to for the reply form
 */
function setupReplyEnterBehavior(cheepId: number): void {
    const textarea = document.getElementById(`reply-textarea-${cheepId}`) as HTMLTextAreaElement | null;
    const form = document.getElementById(`reply-form-${cheepId}`) as HTMLFormElement | null;

    if (!textarea || !form) return;

    // Avoid adding duplicate listeners
    if ((textarea as any)._enterListenerAttached) return;
    (textarea as any)._enterListenerAttached = true;

    textarea.addEventListener("keydown", function (event) {
        if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault();
            submitFormPreservingScroll(form);
        }
    });
}

function submitFormPreservingScroll(form: HTMLFormElement): void {
    persistScrollPosition();

    if (typeof form.requestSubmit === "function") {
        form.requestSubmit();
    } else {
        form.submit();
    }
}

/**
 * Attach a character count listener that updates as more characters are added to the textarea
 * @param cheepId - the textarea form to attach a character count listener
 */
function setupCharcountMonitor(textarea: HTMLTextAreaElement): void {
    // Avoid adding duplicate listeners
    if ((textarea as any)._charcountListenerAttached) return;
    (textarea as any)._charcountListenerAttached = true;

    textarea.addEventListener("input", () => {
        updateCharcount(textarea)
    });

    // Ensure correct charcount
    updateCharcount(textarea)
}

/**
 * Update the charcount associated with a given textarea
 * @param textarea - the textarea's charcount to update
 */
function updateCharcount(textarea: HTMLTextAreaElement): void {
    const maxCharcount = textarea.maxLength;
    const charcount = textarea.value.length;
    const parentId: string = textarea.parentElement.id;
    const charcountElement = document.getElementById(`${parentId}-charcount`);
    charcountElement.innerHTML = `${(maxCharcount - charcount).toString()} characters left`;
}

document.addEventListener('DOMContentLoaded', () => {
    // Setup event handlers
    Array.from(document.getElementsByClassName("post-textarea"),).forEach((element: HTMLTextAreaElement) => {
        setupCharcountMonitor(element);
    });
    Array.from(document.getElementsByClassName("edit-textarea")).forEach((element: HTMLTextAreaElement) => {
        setupCharcountMonitor(element);
    });
    Array.from(document.getElementsByClassName("reply-textarea")).forEach((element: HTMLTextAreaElement) => {
        setupCharcountMonitor(element);
    });
});
