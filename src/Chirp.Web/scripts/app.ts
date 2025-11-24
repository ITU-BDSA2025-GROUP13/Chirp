/**
 * Toggles the reply UI
 * @param cheepId - the ID to query for
 */
function toggleReply(cheepId: number): void {
    const replyFormWrapper = document.getElementById(`reply-form-wrapper-${cheepId}`);
    const replyTextField = document.getElementById(`reply-text-field-${cheepId}`);
    if (!replyFormWrapper || !replyTextField) return;

    if (replyFormWrapper.style.display === "none" || replyFormWrapper.style.display === "") {
        replyFormWrapper.style.display = "block";
        replyTextField.focus();
        setupReplyEnterBehavior(cheepId);
    } else {
        replyFormWrapper.style.display = "none";
    }
}
/**
 * @param {string} cheepId - the cheepId to query for
 */
function editCheep(cheepId: string): void {
    const cheep = document.getElementById(`cheep-${cheepId}`);
    const cheep_text = cheep.querySelector<HTMLElement>("#cheep_text")
    const cheep_edit = cheep.querySelector<HTMLElement>("#cheep_edit")

    if (cheep_text.style.display === "none") {
        cheep_text.style.display = "block";
        cheep_edit.style.display = "none"
    } else {
        cheep_text.style.display = "none";
        cheep_edit.style.display = "block";
    }
}

/**
 * @param cheepId - the cheepId to for the reply form
 */
function setupReplyEnterBehavior(cheepId: number): void {
    const textarea = document.getElementById(`reply-text-field-${cheepId}`) as HTMLTextAreaElement | null;
    const form = document.getElementById(`reply-form-${cheepId}`) as HTMLFormElement | null;

    if (!textarea || !form) return;

    // Avoid adding duplicate listeners
    if ((textarea as any)._enterListenerAttached) return;
    (textarea as any)._enterListenerAttached = true;

    textarea.addEventListener("keydown", function (event) {
        if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault();
            form.submit();
        }
    });
}
