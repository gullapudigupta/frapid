﻿function createCards() {
    function request(kanbanIds, resourceIds) {
        var url = "/api/kanbans/get-by-resources/";
        url += "?kanbanIds=";
        url += kanbanIds.join("&kanbanIds=");

        url += "&resourceIds=";
        url += resourceIds.join("&resourceIds=");

        return getAjaxRequest(url);
    };

    if (!window.scrudjson) {
        return;
    };

    if (!window.kanbans) {
        return;
    };

    var keyField = (window.scrudFactory.card.keyField || getIdField());
    var resourceIds = Enumerable.From(window.scrudjson).Select(function (x) { return x[toProperCase(keyField)]; }).ToArray();
    var kanbanIds = Enumerable.From(window.kanbans).Select(function (x) { return x.KanbanId; }).ToArray();

    if(!window.scrudjson.length){
        return;
    };

    var ajax = request(kanbanIds, resourceIds);

    ajax.success(function (response) {
        $(".kanban.holder").html("");

        $.each(window.scrudjson, function (i, v) {
            var key = getCardKey(v);
            var kanbanDetail = (Enumerable.From(response).Where(function(detail) {
                    return detail.ResourceId.toString() === key.toString();
            }).ToArray()[0] || new Object());

            createCard(this, key, kanbanDetail);
        });

        makeSortable();
        makeRatable();
        //displayFlaggedCards();
    });
};