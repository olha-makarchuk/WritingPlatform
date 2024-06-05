import { Component } from '@angular/core';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})

export class HomeComponent {
  src = 'https://booksinshops.blob.core.windows.net/books-in-shop/Макарчук_Ольга_ІК-12_МКР-2 (2).pdf';
  currentPage = 1;
  totalPages = 10; // Загальна кількість сторінок у PDF

  goToPage() {
    // Перевірка на коректність значення сторінки
    if (this.currentPage < 1) {
      this.currentPage = 1;
    } else if (this.currentPage > this.totalPages) {
      this.currentPage = this.totalPages;
    }
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }
}
