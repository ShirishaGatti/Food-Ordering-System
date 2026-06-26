$(document).on("click", ".edit-order-btn", function () {
    var id = $(this).data("id");

    $.ajax({
        url: "/Order/SaveOrder",
        type: "GET",
        data: { id: id },
        success: function (response) {
            $("#orderModalContainer").html(response);
          
            recalcTotal();
              new bootstrap.Modal(
                document.getElementById("orderModal"),
                {
                    backdrop: "static",
                    keyboard: false
                }
            ).show();
              showToast("Order updated successfully!");
        },
        error: function () { alert("Failed to load order."); }
    });
});

$(document).on("change", ".item-dropdown", function () {
    var price = parseFloat($(this).find("option:selected").data("price")) || 0;
    $(this).closest("tr").find(".price-box").val(price.toFixed(2));
    recalcTotal();
});

$(document).on("input", ".qty-box", function () {
    recalcTotal();
});

$(document).on("click", "#addItemRowBtn", function () {
    var allItems = JSON.parse(
        document.getElementById("allItemsJson").textContent
    );

    var options = allItems.map(function (x) {
        return '<option value="' + x.ItemId + '" data-price="'
            + x.Price + '">' + x.ItemName + '</option>';
    }).join("");

    var firstPrice = allItems.length ? allItems[0].Price.toFixed(2) : "0.00";

    var row = '<tr>'
        + '<td><select class="form-select item-dropdown">' + options + '</select></td>'
        + '<td><input type="number" class="form-control qty-box" min="1" value="1"/></td>'
        + '<td><input type="text" class="form-control price-box" readonly value="' + firstPrice + '"/></td>'
        + '<td class="text-center"><button type="button" '
        + 'class="btn btn-sm btn-outline-danger remove-row-btn">✕</button></td>'
        + '</tr>';

    $("#itemsTable tbody").append(row);
    recalcTotal();
});

$(document).on("click", ".remove-row-btn", function () {
    $(this).closest("tr").remove();
    recalcTotal();
});

$(document).on("click", "#saveOrderBtn", function () {

    // Collect items from table rows
    var items = [];
    $("#itemsTable tbody tr").each(function () {
        items.push({
            ItemId: parseInt($(this).find(".item-dropdown").val()) || 0,
            Quantity: parseInt($(this).find(".qty-box").val()) || 0,
            Price: parseFloat($(this).find(".price-box").val()) || 0
        });
    });

    var payload = {
        OrderId: $("#hOrderId").val(),
        UserId: $("#hUserId").val(),
        OrderDate: $("#hOrderDate").val(),   // "yyyy-MM-dd" string — MVC binds fine
        Items: items
    };

    $.ajax({
        url: "/Order/SaveOrder",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        success: function () {
            bootstrap.Modal.getInstance(
                document.getElementById("orderModal")
            ).hide();
            showToast("Order saved successfully!");
            location.reload();
          
        },
        error: function (xhr) {
            alert("Save failed: " + xhr.responseText);
        }
    });
});
$(document).on("click", "#addOrderBtn", function () {

    $.ajax({
        url: "/Order/SaveOrder",
        type: "Get",
        success: function (response) {

            $("#orderModalContainer").html(response);

            recalcTotal();

            new bootstrap.Modal(
                document.getElementById("orderModal"),
                {
                    backdrop: "static",
                    keyboard: false
                }
            ).show();
        },
        error: function () {
            alert("Failed to load order form.");
        }
    });
});
function showToast(message) {
    $("#toastBody").text(message);

    var toast = new bootstrap.Toast(
        document.getElementById("liveToast")
    );

    toast.show();
}
function recalcTotal() {
    var total = 0;
    $("#itemsTable tbody tr").each(function () {
        var price = parseFloat($(this).find(".price-box").val()) || 0;
        var qty = parseInt($(this).find(".qty-box").val()) || 0;
        total += price * qty;
    });
    $("#orderTotal").text(total.toFixed(2));
}

var selectedOrderId = 0;

$(document).on("click", ".delete-order-btn", function () {
    //alert("Delete failed.");
    selectedOrderId = $(this).data("id");

    new bootstrap.Modal(
        document.getElementById("deleteOrderModal")
    ).show();

});

$(document).on("click", "#confirmDeleteOrderBtn", function () {
    //alert("Delete failed.");
    $.ajax({
        url: "/Order/DeleteOrder",
        type: "POST",
        data: {
            orderId: selectedOrderId
        },
        success: function (response) {

            bootstrap.Modal.getInstance(
                document.getElementById("deleteOrderModal")
            ).hide();

            showToast("Order deleted successfully!");

            setTimeout(function () {
                location.reload();
            }, 1000);
        },
        error: function () {
            alert("Delete failed.");
        }
    });

});