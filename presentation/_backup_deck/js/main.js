/**
 * 말랑 블라스트 (Mallang Blast) 발표 자료 인터랙션 스크립트
 * - PDF 변환 / 인쇄 트리거
 * - 슬라이드 내비게이션 및 스크롤 스파이
 * - 키보드 슬라이드 이동 (화살표 좌/우, PageUp/PageDown)
 * - 인게임 스크린샷 썸네일 전환 갤러리
 */

// 스크린샷 이미지 전환 함수 (전역 노출)
function changeScreenshot(src, thumbElement) {
  const mainImg = document.getElementById('mainGameplayImg');
  if (mainImg) {
    mainImg.style.opacity = '0.4';
    setTimeout(() => {
      mainImg.src = src;
      mainImg.style.opacity = '1';
    }, 150);
  }

  const thumbs = document.querySelectorAll('.screenshot-thumbs .thumb');
  thumbs.forEach(t => t.classList.remove('active'));
  if (thumbElement) {
    thumbElement.classList.add('active');
  }
}

document.addEventListener('DOMContentLoaded', () => {
  const slides = document.querySelectorAll('.slide');
  const navDots = document.querySelectorAll('.nav-dot');
  const printBtn = document.getElementById('btnPrintPdf');

  // 1. PDF 인쇄 버튼 동작
  if (printBtn) {
    printBtn.addEventListener('click', () => {
      window.print();
    });
  }

  // 2. 슬라이드 스크롤 스파이 & 내비게이션 닷 활성화
  const observerOptions = {
    root: null,
    rootMargin: '0px',
    threshold: 0.5
  };

  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        const slideId = entry.target.id;
        navDots.forEach(dot => {
          if (dot.getAttribute('data-target') === slideId) {
            dot.classList.add('active');
          } else {
            dot.classList.remove('active');
          }
        });
      }
    });
  }, observerOptions);

  slides.forEach(slide => observer.observe(slide));

  // 3. 내비게이션 닷 클릭 이동
  navDots.forEach(dot => {
    dot.addEventListener('click', () => {
      const targetId = dot.getAttribute('data-target');
      const targetSlide = document.getElementById(targetId);
      if (targetSlide) {
        targetSlide.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    });
  });

  // 4. 키보드 방향키 슬라이드 탐색 지원
  let currentSlideIndex = 0;
  window.addEventListener('keydown', (e) => {
    if (['INPUT', 'TEXTAREA'].includes(document.activeElement.tagName)) return;

    if (e.key === 'ArrowDown' || e.key === 'ArrowRight' || e.key === 'PageDown' || e.key === ' ') {
      if (currentSlideIndex < slides.length - 1) {
        e.preventDefault();
        currentSlideIndex++;
        slides[currentSlideIndex].scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    } else if (e.key === 'ArrowUp' || e.key === 'ArrowLeft' || e.key === 'PageUp') {
      if (currentSlideIndex > 0) {
        e.preventDefault();
        currentSlideIndex--;
        slides[currentSlideIndex].scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    }
  });

  // 스크롤 시 현재 인덱스 동기화
  window.addEventListener('scroll', () => {
    slides.forEach((slide, idx) => {
      const rect = slide.getBoundingClientRect();
      if (rect.top <= window.innerHeight / 2 && rect.bottom >= window.innerHeight / 2) {
        currentSlideIndex = idx;
      }
    });
  });
});
