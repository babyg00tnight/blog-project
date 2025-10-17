$(document).ready(function () {
  console.log("Frontend đã tải xong (theme khoa học).");

  // Hiệu ứng cuộn lên đầu trang (scroll to top)
  let btnTop = $('<button class="btn btn-primary position-fixed bottom-0 end-0 m-3 shadow">↑</button>')
    .hide()
    .appendTo("body")
    .click(() => $("html, body").animate({ scrollTop: 0 }, 500));

  $(window).scroll(function () {
    if ($(this).scrollTop() > 200) btnTop.fadeIn();
    else btnTop.fadeOut();
  });

  // Smooth scroll cho link nội bộ (#id)
  $('a[href^="#"]').on("click", function (e) {
    e.preventDefault();
    const target = this.hash;
    if (target) {
      $("html, body").animate(
        { scrollTop: $(target).offset().top - 70 },
        600
      );
    }
  });
});

