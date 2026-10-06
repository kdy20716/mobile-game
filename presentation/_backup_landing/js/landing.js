/**
 * 말랑 블라스트 (Mallang Blast) 공식 홍보 웹사이트 인터랙션 스크립트
 */

function switchGallery(imageSrc, title, desc, clickedElement) {
  const mainImg = document.getElementById('galleryMainImg');
  const titleEl = document.getElementById('galleryTitle');
  const descEl = document.getElementById('galleryDesc');
  const cards = document.querySelectorAll('.thumb-card');

  // 활성 탭 하이라이트 전환
  cards.forEach(c => c.classList.remove('active'));
  if (clickedElement) {
    clickedElement.classList.add('active');
  }

  // 부드러운 페이드 전환
  if (mainImg) {
    mainImg.style.opacity = '0.2';
    setTimeout(() => {
      mainImg.src = imageSrc;
      mainImg.style.opacity = '1';
    }, 150);
  }

  if (titleEl) titleEl.textContent = title;
  if (descEl) descEl.textContent = desc;
}

document.addEventListener('DOMContentLoaded', () => {
  // 네비게이션 스크롤 블러 효과 강화
  const navbar = document.querySelector('.navbar');
  window.addEventListener('scroll', () => {
    if (window.scrollY > 40) {
      navbar.style.background = 'rgba(15, 10, 35, 0.9)';
      navbar.style.boxShadow = '0 10px 30px rgba(0, 0, 0, 0.4)';
    } else {
      navbar.style.background = 'rgba(22, 16, 46, 0.75)';
      navbar.style.boxShadow = 'none';
    }
  });

  // 히어로 폰 스크린샷 주기적 자동 전환 (생동감)
  const heroScreen = document.getElementById('heroScreenImg');
  const heroShots = [
    'assets/gameplay_shot_3.png',
    'assets/gameplay_shot_4.png',
    'assets/gameplay_shot_1.png',
    'assets/gameplay_shot_2.png'
  ];
  let currentHeroIdx = 0;

  setInterval(() => {
    if (!heroScreen) return;
    currentHeroIdx = (currentHeroIdx + 1) % heroShots.length;
    heroScreen.style.opacity = '0.3';
    setTimeout(() => {
      heroScreen.src = heroShots[currentHeroIdx];
      heroScreen.style.opacity = '1';
    }, 200);
  }, 4500);
});
