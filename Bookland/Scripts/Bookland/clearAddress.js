$(function () {
    $("a#clear-address").on("click", function (event) {
        var target = $(event.target);

        var addressFields = target.siblings(".form-row").children('input').val("");
    });
});