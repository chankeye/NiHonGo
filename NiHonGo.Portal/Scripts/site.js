$(function () {
    var jpLangApp = new Vue({
        el: '#japanese',
        methods: {
            changeLanguageToJP: function () {

                $.ajax({
                    type: 'post',
                    url: '/System/ChangeLanguage',
                    data: {
                        language: "ja-JP"
                    },
                    success: function (data) {
                        if (data.IsSuccess) {
                            location.reload();
                        }
                    }
                });
            }
        }
    })

    var cnLangApp = new Vue({
        el: '#chinese',
        methods: {
            changeLanguageToCN: function () {

                $.ajax({
                    type: 'post',
                    url: '/System/ChangeLanguage',
                    data: {
                        language: "zh-CN"
                    },
                    success: function (data) {
                        if (data.IsSuccess) {
                            location.reload();
                        }
                    }
                });
            }
        }
    })
})

var utilsApp = new Vue({
    methods: {
        toMessage: function (key) {
            var lang = document.cookie.match(new RegExp("(^| )" + "language" + "=([^;]*)(;|$)"));

            if (lang != null && lang[2] == "ja-JP") {
                return langJP[key];
            }
            else {
                return langCN[key];
            }
        },
        getUrlLastSegment: function () {
            var href = window.location.href;
            var id = href.substr(href.lastIndexOf('/') + 1);
            return id.replace("#", "");
        }
    }
})