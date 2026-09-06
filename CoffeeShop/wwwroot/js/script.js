$(document).ready(function () {

    // --- 1. SEPETE ÜRÜN EKLEME ---
    $(document).on('click', '.basket-add-btn', function (e) {
        e.preventDefault();
        let btn = $(this);
        let id = btn.attr("data-id");

        fetch('/Home/AddProductToBasket/' + id, { method: 'POST' })
            .then(response => {
                if (!response.ok) throw new Error("Sepete ekleme başarısız.");
                return response.json();
            })
            .then(data => {
                $(".shop-cart sup.rounded-circle").text(data.count);
                $(".shop-cart a span").text("CART ($" + data.total.toFixed(2) + ")");
            })
            .catch(error => console.error("Hata:", error));
    });

    // --- 2. SUBSCRIBE / QUICK LOGIN ---
    $(document).on('click', '#subscribeBtn', function (e) {
        e.preventDefault();
        console.log("Subscribe butonuna tıklandı!");

        let subEmail = $("#subscribeEmail").val().trim();
        let subMessage = $("#subscribeMessage");
        let subBtn = $(this);

        if (!subEmail) {
            subMessage.text("Please enter a valid email address.").css("color", "#ffc107");
            return;
        }

        subBtn.text("Processing...").prop("disabled", true);

        let params = new URLSearchParams();
        params.append("email", subEmail);

        fetch('/Account/QuickSubscribeLogin', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: params
        })
            .then(response => {
                if (!response.ok) throw new Error("Sunucu hatası: " + response.status);
                return response.json();
            })
            .then(result => {
                console.log("Yanıt:", result);
                subMessage.text(result.message).css("color", result.success ? "#00704A" : "#ffc107");

                if (result.success && result.isLogin) {
                    setTimeout(() => { window.location.reload(); }, 1500);
                } else {
                    subBtn.text("Subscribe").prop("disabled", false);
                    if (result.success) $("#subscribeEmail").val("");
                }
            })
            .catch(error => {
                console.error("HATA:", error);
                subMessage.text("An error occurred. Please try again.").css("color", "red");
                subBtn.text("Subscribe").prop("disabled", false);
            });
    });

    // --- 3. SEPETTEN ÜRÜN SİLME ---
    $(document).on('click', '.delete-basket-btn', function (e) {
        e.preventDefault();
        let id = $(this).closest('button').attr("data-id");

        fetch('/Basket/RemoveFromBasket/' + id, { method: 'POST' })
            .then(response => {
                if (response.ok) window.location.reload();
            })
            .catch(error => console.error("Silme Hatası:", error));
    });

    // --- 4. SEPETTE ÜRÜN ARTIRMA (+) ---
    $(document).on('click', '.increase-basket-btn', function (e) {
        e.preventDefault();
        let id = $(this).closest('button').attr("data-id");

        fetch('/Basket/IncreaseProductCount/' + id, { method: 'POST' })
            .then(response => {
                if (response.ok) window.location.reload();
            })
            .catch(error => console.error("Artırma Hatası:", error));
    });

    // --- 5. SEPETTE ÜRÜN AZALTMA (-) ---
    $(document).on('click', '.decrease-basket-btn', function (e) {
        e.preventDefault();
        let id = $(this).closest('button').attr("data-id");

        fetch('/Basket/DecreaseProductCount/' + id, { method: 'POST' })
            .then(response => {
                if (response.ok) window.location.reload();
            })
            .catch(error => console.error("Azaltma Hatası:", error));
    });

    // --- 6. NAV-BAR VE MOBİL MENÜ ---
    $(document).on('click', '#search', function () {
        $(this).next().toggle();
    });

    $(document).on('click', '#mobile-navbar-close', function () {
        $(this).parent().removeClass("active");
    });

    $(document).on('click', '#mobile-navbar-show', function () {
        $('.mobile-navbar').addClass("active");
    });

    // --- 7. SLIDER / CAROUSEL ---
    // if ($(".slider").length) {
    //     $(".slider").owlCarousel({
    //         items: 1,
    //         loop: true,
    //         autoplay: true
    //     });
    // }

    // if ($(".instagram").length) {
    //     $(".instagram").owlCarousel({
    //         items: 4,
    //         loop: true,
    //         autoplay: true,
    //         responsive: {
    //             0: { items: 1 },
    //             576: { items: 2 },
    //             768: { items: 3 },
    //             992: { items: 4 }
    //         }
    //     });
    // }
});