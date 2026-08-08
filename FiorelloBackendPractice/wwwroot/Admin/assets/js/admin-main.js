"use strict";

document.addEventListener("click", function (e) {
    // Slider Silme
    if (e.target.classList.contains("delete-slider-btn")) {
        let btn = e.target;
        let id = btn.getAttribute("data-id");

        fetch(`/Admin/Slider/Delete/${id}`, {
            method: "POST"
        }).then(response => {
            if (response.ok) {
                btn.closest("tr").remove();
            } else {
                alert("Failed to Delete!");
            }
        });
    }

    // Category Silme
    if (e.target.classList.contains("delete-btn")) {
        let btn = e.target;
        let id = btn.getAttribute("data-id");

        fetch(`/Admin/Category/Delete/${id}`, {
            method: "POST"
        }).then(response => {
            if (response.ok) {
                btn.closest("tr").remove();
            } else {
                alert("Failed to Delete!");
            }
        });
    }
});