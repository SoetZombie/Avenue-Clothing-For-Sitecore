﻿var VariantPicker = (function () {

    // declared with `var`, must be "private"
    var classSelector = ".js-variant-picker";

    var getVariantNameValueDictionary = function () {
        var data = {};

        $(classSelector)
            .each(function () {
                var $picker = $(this);
                var productSku = $picker.data("product-sku");

                if (currentProductSku != productSku) {
                    return;
                }

                var dataVariantName = $picker.data("variant-name");
                var variantValue = $picker.val();

                data[dataVariantName] = variantValue;
            });

        return data;
    };

    var productVariantChanged = function (event, data) {
        var $picker = $(this);

        var productSku = $picker.data("product-sku");
        if (productSku !== data.productSku) {
            return;
        }

        //TODO: disable options that are no longer possible -- server side get possible options
    };

    var publicScope = {
        init: function (rootSelector) {
            $(rootSelector).find(classSelector)
                .on("product-variant-changed", productVariantChanged)//TODO: Avoid Event Storm create a latch?
                .change(function () {
                    var $picker = $(this);
                    var productSku = $picker.data("product-sku");
                    var variantExistsUrl = $picker.data("variant-exists-url");

                    var variantNameValueDictionary = getVariantNameValueDictionary();

                    $.ajax({
                        type: "POST",
                        url: variantExistsUrl,
                        data:
                        {
                            ProductSku: productSku,
                            VariantNameValueDictionary: variantNameValueDictionary
                        },
                        dataType: "json"
                    }
                    .done(function (data) {
                        var productVariantSku = data.ProductVariantSku;

                        $(document).trigger("product-variant-changed",
                        {
                            productSku: productSku,
                            productVariantSku: productVariantSku
                        });
                    })
                    .fail(function () {
                        alert("Whoops...");
                    })
                    .always(function () {
                        //No-op
                    }));
                });
        }
    };

    return publicScope;

})();