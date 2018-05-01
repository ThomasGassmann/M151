'use strict';

window.Taobao = window.Taobao || {};

(function (namespace, $, undefined) {
    $(function () {
        $('.date-picker').datepicker({
            format: 'DD.MM.YYYY'
        });
    });

    namespace.addRetailerProduct = function (id) {
        var children = $('.retailer-products').children().length;
        $.ajax({
            method: 'GET',
            url: '/RetailerProducts/New/' + id.toString() + '?amount=' + children.toString()
        }).done(function (data) {
            $('.retailer-products').append(data);
        });
    };

    namespace.removeRetailerProduct = function (id) {
        var children = $('.retailer-products').children();
        for (var i = 0; i < children.length; i++) {
            var child = children[i];
            if (child.innerHTML.indexOf('[' + id.toString() + '].ProductId') > -1) {
                child.remove();
                return;
            }
        }
    };
})(window.Taobao, jQuery);
