// Click event handler intended for the toggling of hiding and showing a 
// specific category's descendant categories
function toggleCategoryDescendantsVisibility() {
    $("a.category-link").on("click", function (event) {
        var categoryLink = $(event.target);
        var children = categoryLink.parent().siblings(".children");

        if (children.css("display") === "none") {
            categoryLink.text("[-]");
            children.slideDown("fast");
        } else {
            categoryLink.text("[+]");
            children.slideUp("fast");
        }

        event.preventDefault();
    });
}

// Click event handler that captures clicks on a category to update the
// user-selected category, which also reveals that category's action (e.g. Update) links
function showCategoryActionLinks() {
    $("div.parent").on("click", function (event) {
        var selectedParent = $(event.target);
        var selectedParentActions = selectedParent.children("a.actions-link");
        var prevSelectedParent = $(".selected-category");
        var prevSelectedParentActions = prevSelectedParent.children("a.actions-link");

        if (selectedParentActions.css("display") === "none") {
            prevSelectedParentActions.hide();
            prevSelectedParent.removeClass("selected-category");

            selectedParent.addClass("selected-category");
            selectedParentActions.show();
        }
    });
}