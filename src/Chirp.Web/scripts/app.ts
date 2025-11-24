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
            form.submit();
        }
    });
}

/**
 * Toggles the reply UI
 * @param cheepId - the ID to query for
 */
function toggleReply(cheepId: number): void {
    const replyFormWrapper = document.getElementById(`reply-form-wrapper-${cheepId}`);
    const replyTextField = document.getElementById(`reply-text-field-${cheepId}`);
    if (!replyFormWrapper || !replyTextField) return;

    if (replyFormWrapper.style.display === 'none' || replyFormWrapper.style.display === '') {
        replyFormWrapper.style.display = 'block';
        replyTextField.focus();
        setupReplyEnterBehavior(cheepId);
    } else {
        replyFormWrapper.style.display = 'none';
    }
}

/**
 * @param {string} cheepId - the cheepId to query for
 */
function editCheep(cheepId: number): void {
    const cheep = document.getElementById(`cheep-${cheepId}`);
    const cheepText = cheep.querySelector<HTMLElement>("#cheep-text-" + cheepId);
    const cheepEdit = cheep.querySelector<HTMLElement>("#cheep-edit-" + cheepId);
    const cheepEditInput = cheep.querySelector<HTMLInputElement>("#cheep-edit-input-" + cheepId);
    if (!cheepText || !cheepEdit || !cheepEditInput) return;

    if (cheepText.style.display === "none") {
        cheepText.style.display = "block";
        cheepEdit.style.display = "none"   
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
