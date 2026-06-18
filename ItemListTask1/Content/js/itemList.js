"use strict";

// ── Config (injected by Razor) ─────────────────────────────────────────────
const cfg = window.ItemListConfig;

// ── State ──────────────────────────────────────────────────────────────────
let currentPage = cfg.initialPage;
let sortField = "ItemId";
let sortDirection = "ASC";
let deleteModal;

// ── Loading helpers ────────────────────────────────────────────────────────
function showLoading() {
    $("#submitBtn, #resetBtn").prop("disabled", true);
    $("#tableContainer").hide();
    $("#loadingOverlay").fadeIn(150);
}

function hideLoading() {
    $("#submitBtn, #resetBtn").prop("disabled", false);
    $("#loadingOverlay").fadeOut(150, function () {
        $("#tableContainer").show();
    });
}

// ── Core data loader ───────────────────────────────────────────────────────
function loadData(pageNumber) {
    currentPage = pageNumber;

    const data = {
        pageNumber: pageNumber,
        pageSize: $("#pageSize").val(),
        SelectedCategoryIds: $("#SelectedCategoryIds").val(),
        RestaurantName: $("#RestaurantName").val(),
        status: $("#Status").val(),
        SortField: sortField,
        SortDirection: sortDirection,
    };

    showLoading();

    $.ajax({
        url: cfg.getItemsUrl,
        method: "POST",
        data: data,
        success: function (res) { $("#tableContainer").html(res); },
        error: function (err) { console.error(err); alert("Error loading data"); },
        complete: function () { hideLoading(); }
    });
}

// ── Document ready ─────────────────────────────────────────────────────────
$(document).ready(function () {

    // Hide table initially
    $("#tableContainer").hide();

    // Form submit → reload page 1
    $("#itemListForm").submit(function (e) {
        e.preventDefault();
        currentPage = 1;
        loadData(currentPage);
    });

    // Reset button
    $("#resetBtn").click(function () {
        sessionStorage.removeItem("itemFilters");
        $("#itemListForm")[0].reset();
        $("#SelectedCategoryIds").val(null).trigger("change");
        currentPage = 1;
        loadData(currentPage);
    });

    // Select2 for multi-category
    $("#SelectedCategoryIds").select2({
        placeholder: "Select categories",
        closeOnSelect: false,
        width: "100%",
        templateSelection: function (data) { return data.text; }
    });

    // Collapse extra chips after 2
    $("#SelectedCategoryIds").on("change", function () {
        const $choices = $(".select2-selection__choice");
        const total = $choices.length;

        $(".more-count").remove(); // clear old badge

        if (total > 2) {
            $choices.each(function (i) { $(this).toggle(i < 2); });
            $choices.eq(1).after(`<span class="more-count"> +${total - 2} more</span>`);
        } else {
            $choices.show();
        }
    });

    // Bootstrap delete modal (static)
    deleteModal = new bootstrap.Modal(
        document.getElementById("deleteitemModal"),
        { backdrop: "static", keyboard: false }
    );

    // Initial load
    loadData(currentPage);
});

// ── Edit / Add button ──────────────────────────────────────────────────────
$(document).on("click", ".edit-btn, #AddBtn", function () {
    const id = $(this).attr("data-id");

    $.get(cfg.saveItemUrl, { id: id }, function (res) {
        if (res.success) {
            // modal already closed upstream; just refresh
            loadData(currentPage);
        } else {
            $("#modalContainer").html(res);
        }
    });
});

// ── Delete: open modal ─────────────────────────────────────────────────────
$(document).on("click", ".delete-btn", function () {
    $("#ItemId").val($(this).data("id"));
    $("#ItemName").val($(this).data("name"));
    deleteModal.show();
});

// ── Delete: confirm ────────────────────────────────────────────────────────
$(document).on("click", "#confirm-delete-btn", function () {
    $.ajax({
        url: cfg.deleteItemUrl,
        type: "POST",
        data: { ItemId: $("#ItemId").val() },
        success: function () {            
            deleteModal.hide();
            loadData(currentPage);
            showToast("Item deleted successfully!");
        }
    });
});

// ── Column sort ────────────────────────────────────────────────────────────
$(document).on("click", ".sortable", function () {
    const field = $(this).data("field");

    if (sortField === field) {
        sortDirection = sortDirection === "ASC" ? "DESC" : "ASC";
    } else {
        sortField = field;
        sortDirection = "ASC";
    }

    loadData(1);
});