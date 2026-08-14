$(document).ready(function () {

    // --- 1. SEPETE ÜRÜN EKLEME (Alert Kaldırıldı, Temiz Kod) ---
    $(document).on('click', '.basket-add-btn', function (e) {
        e.preventDefault();

        let btn = $(this);
        let id = btn.attr("data-id");

        fetch(`/Home/AddProductToBasket/${id}`, {
            method: 'POST'
        })
            .then(response => {
                if (!response.ok) throw new Error("Sepete ekleme başarısız.");
                return response.json();
            })
            .then(data => {
                // Sadece arayüzdeki sepet sayı ve tutarını günceller, alert vermez
                $(".shop-cart sup.rounded-circle").text(data.count);
                $(".shop-cart a span").text(`CART ($${data.total.toFixed(2)})`);
            })
            .catch(error => console.error("Hata:", error));
    });

    // --- 2. SEPETTEN ÜRÜN SİLME ---
    $(document).on('click', '.delete-basket-btn', function (e) {
        e.preventDefault();

        let btn = $(this);
        let id = btn.attr("data-id");

        fetch(`/Basket/RemoveFromBasket/${id}`, {
            method: 'POST'
        })
            .then(response => {
                if (response.ok) {
                    window.location.reload();
                }
            })
            .catch(error => console.error("Silme Hatası:", error));
    });

    // --- DİĞER FONKSİYONLAR (Arama, Menü, Slider vb.) ---

    $(document).on('click', '#search', function () {
        $(this).next().toggle();
    });

    $(document).on('click', '#mobile-navbar-close', function () {
        $(this).parent().removeClass("active");
    });

    $(document).on('click', '#mobile-navbar-show', function () {
        $('.mobile-navbar').addClass("active");
    });

    $(document).on('click', '.mobile-navbar ul li a', function () {
        if ($(this).children('i').hasClass('fa-caret-right')) {
            $(this).children('i').removeClass('fa-caret-right').addClass('fa-sort-down');
        } else {
            $(this).children('i').removeClass('fa-sort-down').addClass('fa-caret-right');
        }
        $(this).parent().next().slideToggle();
    });

    if ($(".slider").length) {
        $(".slider").owlCarousel({
            items: 1,
            loop: true,
            autoplay: true
        });
    }

    $(document).on('click', '.categories', function (e) {
        e.preventDefault();
        $(this).next().next().slideToggle();
    });

    $(document).on('click', '.category li a', function (e) {
        e.preventDefault();
        let category = $(this).attr('data-id');
        let products = $('.product-item');

        products.each(function () {
            if (category == 'all' || category == $(this).attr('data-id')) {
                $(this).parent().fadeIn();
            } else {
                $(this).parent().hide();
            }
        });
    });

    $(document).on('click', '.question', function () {
        $(this).siblings('.question').children('i').removeClass('fa-minus').addClass('fa-plus');
        $(this).siblings('.answer').not($(this).next()).slideUp();
        $(this).children('i').toggleClass('fa-plus').toggleClass('fa-minus');
        $(this).next().slideToggle();
        $(this).siblings('.active').removeClass('active');
        $(this).toggleClass('active');
    });

    $(document).on('click', 'ul li', function () {
        $(this).siblings('.active').removeClass('active');
        $(this).addClass('active');
        let dataId = $(this).attr('data-id');
        $(this).parent().next().children('p.active').removeClass('active');

        $(this).parent().next().children('p').each(function () {
            if (dataId == $(this).attr('data-id')) {
                $(this).addClass('active');
            }
        });
    });

    $(document).on('click', '.tab4 ul li', function () {
        $(this).siblings('.active').removeClass('active');
        $(this).addClass('active');
        let dataId = $(this).attr('data-id');
        $(this).parent().parent().next().children().children('p.active').removeClass('active');

        $(this).parent().parent().next().children().children('p').each(function () {
            if (dataId == $(this).attr('data-id')) {
                $(this).addClass('active');
            }
        });
    });

    if ($(".instagram").length) {
        $(".instagram").owlCarousel({
            items: 4,
            loop: true,
            autoplay: true,
            responsive: {
                0: { items: 1 },
                576: { items: 2 },
                768: { items: 3 },
                992: { items: 4 }
            }
        });
    }

    if ($(".say").length) {
        $(".say").owlCarousel({
            items: 1,
            loop: true,
            autoplay: true
        });
    }
});