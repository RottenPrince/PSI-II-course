document.getElementById("submitButton").addEventListener("click", function () {
    var selectedAnswer = $("input[name='answer']:checked").attr("value");

    if (selectedAnswer !== null) {
        $.ajax({
            type: "POST",
            url: "https://localhost:7016/api/QuestionAPI/CheckAnswer",
            data: JSON.stringify({
                room: getRoomName(),
                name: getQuestionName(),
                answer: parseInt(selectedAnswer)
            }),
            success: function (data) {
                $("#answerMessage").text(data === true ? "You are Right!" : "WRONG!");
            }, // TODO error handling
            contentType: "application/json",
            dataType: "json"
        });
    }
});
