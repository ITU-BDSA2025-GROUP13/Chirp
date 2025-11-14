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
