$(function () {
    $("input.card-number").on("input propertychange paste", function (event) {
        var target = $(event.target);
        if (target.val().length === 4) {
            $(target).next("input.card-number").focus();
        }
    });
});