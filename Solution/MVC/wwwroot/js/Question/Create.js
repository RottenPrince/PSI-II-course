$(document).ready(function () {
    // Add Answer Option
    $("#addOption").click(function () {
        var optionCount = $("input[type='radio'][name^='CorrectAnswerIndex']").length;

        var optionHtml = `
                <div class="answerOption">
                    <input type="radio" name="CorrectAnswerIndex" value="${optionCount}" />
                    <input type="text" class="answerOptionBox" name="AnswerOptions[${optionCount}].OptionText" value="" />
                    <button type="button" class="removeOption">Remove</button>
                </div>`;

        $("#answerOptionsContainer").append(optionHtml);

        // Update the values of existing radio buttons
        $("input[type='radio'][name^='CorrectAnswerIndex']").each(function (index) {
            $(this).val(index);
        });
    });

    // Delete
    $("#answerOptionsContainer").on("click", ".removeOption", function () {
        $(this).parent().remove();

        $("input[type='radio'][name^='CorrectAnswerIndex']").each(function (index) {
            $(this).val(index);
        });
    });

    $("form").submit(function (e) {
        var selectedRadio = $("input[name='CorrectAnswerIndex']:checked");
        var answerTextBoxes = $("input[name^='AnswerOptions'][name$='OptionText']");
        var titleTextBox = $("input[name='Title']");

        var isAnyAnswerEmpty = answerTextBoxes.toArray().some(function (textBox) {
            return textBox.value.trim() === "";
        });

        if (selectedRadio.length === 0 || isAnyAnswerEmpty || titleTextBox.val().trim() === "") {
            e.preventDefault();
            $("#submit-check").html("Please fill in all fields and select a correct answer.");
        }
    });
});
