function changedNone() {
    var oldClasses = document.querySelectorAll("li.nav-item.li-custom");
    if (oldClasses[0].classList.contains("li-custom")) {
        for (var i = 0; i < oldClasses.length; i++) {
            oldClasses[i].classList.add("d-none");
        };
    }
}
function changedBlock() {
    var oldClasses = document.querySelectorAll("li.nav-item.li-custom.d-none");
    if (oldClasses[0].classList.contains("d-none")) {
        for (var i = 0; i < oldClasses.length; i++) {
            oldClasses[i].classList.remove("d-none");
        };
    }
}

function toggleCheckbox(selectElement) {
    var checkboxDiv = document.getElementById("checkboxDiv");
    if (selectElement.value === "2") {
        checkboxDiv.style.display = "block";
    } else {
        checkboxDiv.style.display = "none";
    }
}