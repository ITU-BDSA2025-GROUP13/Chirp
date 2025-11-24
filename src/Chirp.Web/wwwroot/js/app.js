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
});
window.addEventListener('pageshow', event => {
    if (event.persisted) {
        restoreScrollPosition();
    }
});
/**
 * Toggles the reply UI
 * @param cheepId - the ID to query for
 */
function toggleReply(cheepId) {
    const replyFormWrapper = document.getElementById(`reply-form-wrapper-${cheepId}`);
    const replyTextField = document.getElementById(`reply-text-field-${cheepId}`);
    if (!replyFormWrapper || !replyTextField)
        return;
    if (replyFormWrapper.style.display === "none" || replyFormWrapper.style.display === "") {
        replyFormWrapper.style.display = "block";
        replyTextField.focus();
        setupReplyEnterBehavior(cheepId);
    }
    else {
        replyFormWrapper.style.display = "none";
    }
    setupScrollPreservation();
}
/**
 * @param {string} cheepId - the cheepId to query for
 */
function editCheep(cheepId) {
    const cheep = document.getElementById(`cheep-${cheepId}`);
    const cheepText = cheep.querySelector("#cheep-text-" + cheepId);
    const cheepEdit = cheep.querySelector("#cheep-edit-" + cheepId);
    const cheepEditInput = cheep.querySelector("#cheep-edit-input-" + cheepId);
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
    const textarea = document.getElementById(`reply-text-field-${cheepId}`);
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
//# sourceMappingURL=app.js.map