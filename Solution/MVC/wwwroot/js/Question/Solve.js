document.addEventListener("DOMContentLoaded", function () {
    
    var form = document.getElementById("form");

    form.addEventListener("submit", function (event) {
        var radioButtons = document.querySelectorAll("input[name='selectedOption']");

        var isAnyChecked = Array.from(radioButtons).some(function (radioButton) {
            return radioButton.checked;
        });

        if (!isAnyChecked) {
            var errorMessage = document.getElementById("answerMessage");
            errorMessage.textContent = "Please select the Answer.";

            event.preventDefault();
        }
    });
});
