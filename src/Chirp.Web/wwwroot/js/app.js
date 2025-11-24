var _a;
const scrollStorageKey = `chirp-scroll:${window.location.pathname}${window.location.search}`;
const canUseSessionStorage = (() => {
    try {
        if (typeof sessionStorage === "undefined")
            return false;
        const testKey = "__chirp_scroll_test__";
        sessionStorage.setItem(testKey, "1");
        sessionStorage.removeItem(testKey);
        return true;
    }
    catch (_a) {
        return false;
    }
})();
const canUseLocalStorage = (() => {
    try {
        if (typeof localStorage === "undefined")
            return false;
        const testKey = "__chirp_theme_test__";
        localStorage.setItem(testKey, "1");
        localStorage.removeItem(testKey);
        return true;
    }
    catch (_a) {
        return false;
    }
})();
const themeStorageKey = "chirp-theme";
const defaultLightHref = "/css/colors-light-theme.css";
const defaultDarkHref = "/css/colors-dark-theme.css";
function getStoredTheme() {
    if (!canUseLocalStorage)
        return null;
    const storedValue = localStorage.getItem(themeStorageKey);
    if (storedValue === "light" || storedValue === "dark")
        return storedValue;
    return null;
}
function storeTheme(theme) {
    if (!canUseLocalStorage)
        return;
    localStorage.setItem(themeStorageKey, theme);
}
function applyTheme(theme) {
    var _a, _b;
    const themeLink = document.getElementById("theme-stylesheet");
    if (!themeLink)
        return;
    const lightHref = (_a = themeLink.dataset.lightHref) !== null && _a !== void 0 ? _a : defaultLightHref;
    const darkHref = (_b = themeLink.dataset.darkHref) !== null && _b !== void 0 ? _b : defaultDarkHref;
    themeLink.href = theme === "dark" ? darkHref : lightHref;
    document.documentElement.setAttribute("data-theme", theme);
    const toggleButton = document.getElementById("themeToggleButton");
    if (toggleButton) {
        updateToggleButtonAppearance(toggleButton, theme);
    }
}
function updateToggleButtonAppearance(button, theme) {
    const icon = button.querySelector("i");
    if (icon) {
        icon.classList.remove("fa-sun-o", "fa-moon-o");
        icon.classList.add(theme === "dark" ? "fa-sun-o" : "fa-moon-o");
    }
    const ariaLabel = theme === "dark" ? "Switch to light theme" : "Switch to dark theme";
    button.setAttribute("aria-label", ariaLabel);
}
const initialTheme = (_a = getStoredTheme()) !== null && _a !== void 0 ? _a : "dark";
applyTheme(initialTheme);
function setupPostEnterBehavior() {
    const textarea = document.getElementById('post-text-field');
    const form = document.getElementById('post-form');
    if (!textarea || !form)
        return;
    // Avoid adding duplicate listeners
    if (textarea._enterListenerAttached)
        return;
    textarea._enterListenerAttached = true;
    textarea.addEventListener("keydown", function (event) {
        if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault();
            submitFormPreservingScroll(form);
        }
    });
}
function persistScrollPosition() {
    if (!canUseSessionStorage)
        return;
    sessionStorage.setItem(scrollStorageKey, window.scrollY.toString());
}
function restoreScrollPosition() {
    if (!canUseSessionStorage)
        return;
    const storedValue = sessionStorage.getItem(scrollStorageKey);
    if (!storedValue)
        return;
    const scrollY = Number(storedValue);
    sessionStorage.removeItem(scrollStorageKey);
    if (Number.isNaN(scrollY))
        return;
    window.requestAnimationFrame(() => {
        window.scrollTo({ top: scrollY, left: window.scrollX });
    });
}
function setupScrollPreservation() {
    if (!canUseSessionStorage)
        return;
    const forms = document.querySelectorAll('form[data-preserve-scroll]');
    forms.forEach(form => {
        if (form._scrollPreserverAttached)
            return;
        form.addEventListener('submit', persistScrollPosition);
        form._scrollPreserverAttached = true;
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
function setupThemeToggle(currentTheme) {
    const toggleButton = document.getElementById('themeToggleButton');
    if (!toggleButton)
        return;
    if (toggleButton._themeListenerAttached)
        return;
    toggleButton._themeListenerAttached = true;
    updateToggleButtonAppearance(toggleButton, currentTheme);
    toggleButton.addEventListener('click', () => {
        const activeTheme = document.documentElement.getAttribute('data-theme') === 'dark' ? 'dark' : 'light';
        const selectedTheme = activeTheme === 'dark' ? 'light' : 'dark';
        applyTheme(selectedTheme);
        storeTheme(selectedTheme);
    });
}
/**
 * Toggles the reply UI
 * @param cheepId - the ID to query for
 */
function toggleReply(cheepId) {
    const replyFormWrapper = document.getElementById(`reply-form-wrapper-${cheepId}`);
    const replyTextField = document.getElementById(`reply-textarea-${cheepId}`);
    if (!replyFormWrapper || !replyTextField)
        return;
    if (replyFormWrapper.style.display === 'none' || replyFormWrapper.style.display === '') {
        replyFormWrapper.style.display = 'block';
        replyTextField.focus();
        setupReplyEnterBehavior(cheepId);
    }
    else {
        replyFormWrapper.style.display = 'none';
    }
    setupScrollPreservation();
}
/**
 * @param {string} cheepId - the cheepId to query for
 */
function editCheep(cheepId) {
    const cheep = document.getElementById(`cheep-${cheepId}`);
    if (!cheep)
        return;
    const cheepText = cheep.querySelector(`#cheep-text-${cheepId}`);
    const cheepEdit = cheep.querySelector(`#cheep-edit-${cheepId}`);
    const cheepEditInput = cheep.querySelector(`#cheep-edit-input-${cheepId}`);
    if (!cheepText || !cheepEdit || !cheepEditInput)
        return;
    if (cheepText.style.display === "none") {
        cheepText.style.display = "block";
        cheepEdit.style.display = "none";
    }
    else {
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
function setupReplyEnterBehavior(cheepId) {
    const textarea = document.getElementById(`reply-textarea-${cheepId}`);
    const form = document.getElementById(`reply-form-${cheepId}`);
    if (!textarea || !form)
        return;
    // Avoid adding duplicate listeners
    if (textarea._enterListenerAttached)
        return;
    textarea._enterListenerAttached = true;
    textarea.addEventListener("keydown", function (event) {
        if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault();
            submitFormPreservingScroll(form);
        }
    });
}
function submitFormPreservingScroll(form) {
    persistScrollPosition();
    if (typeof form.requestSubmit === "function") {
        form.requestSubmit();
    }
    else {
        form.submit();
    }
}
/**
 * Attach a character count listener that updates as more characters are added to the textarea
 * @param cheepId - the textarea form to attach a character count listener
 */
function setupCharcountMonitor(textarea) {
    // Avoid adding duplicate listeners
    if (textarea._charcountListenerAttached)
        return;
    textarea._charcountListenerAttached = true;
    textarea.addEventListener("input", () => {
        updateCharcount(textarea);
    });
    // Ensure correct charcount
    updateCharcount(textarea);
}
/**
 * Update the charcount associated with a given textarea
 * @param textarea - the textarea's charcount to update
 */
function updateCharcount(textarea) {
    const maxCharcount = textarea.maxLength;
    const charcount = textarea.value.length;
    const parentId = textarea.parentElement.id;
    const charcountElement = document.getElementById(`${parentId}-charcount`);
    charcountElement.innerHTML = `${(maxCharcount - charcount).toString()} characters left`;
}
document.addEventListener('DOMContentLoaded', () => {
    // Setup event handlers
    Array.from(document.getElementsByClassName("post-textarea")).forEach((element) => {
        setupCharcountMonitor(element);
    });
    Array.from(document.getElementsByClassName("edit-textarea")).forEach((element) => {
        setupCharcountMonitor(element);
    });
    Array.from(document.getElementsByClassName("reply-textarea")).forEach((element) => {
        setupCharcountMonitor(element);
    });
});
//# sourceMappingURL=app.js.map