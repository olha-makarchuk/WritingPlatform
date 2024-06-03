import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { Genre } from '../../shared/models/genre';
import { Publication, SortPublicationQuery, SortByItem } from '../../shared/models/publication';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.css']
})
export class CatalogComponent implements OnInit {
  genres: Genre[] = [];
  publications: Array<Publication> = [];
  sortQuery: SortPublicationQuery = {
    genreIds: [],
    startPage: 0,
    endPage: 0,
    yearPublication: 0,
    sortByItemId: 1 // Default sort by Rating
  };
  sortByItems: SortByItem[] = []; // Array to store SortByItem objects received from the service
  sortDirection: string = 'asc';
  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.loadGenres();
    this.loadSortByItems(); // Load sort options from the service
    this.sortPublications();
  }

  loadGenres(): void {
    this.userService.getGenres().subscribe(genres => {
      this.genres = genres;
    });
  }

  loadSortByItems(): void {
    this.userService.getAllOrderByItems().subscribe(items => {
      this.sortByItems = items;
    });
  }

  sortPublications(): void {
    this.userService.sortPublications(this.sortQuery, this.sortDirection).subscribe(publications => {
      this.publications = publications;
    });
  }
  
  onSortDirectionChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.sortDirection = target.value;
    this.sortPublications();
  }
  
  onSortChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const selectedItemId = Number(target.value);
    const selectedItem = this.sortByItems.find(item => item.id === selectedItemId);
    if (selectedItem) {
      this.sortQuery.sortByItemId = Number(target.value);
      this.sortPublications();
    }
  }

  onGenreChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const genreId = Number(target.value);
    const index = this.sortQuery.genreIds.indexOf(genreId);
    if (index === -1) {
      // Якщо жанр ще не вибраний, додайте його до масиву
      this.sortQuery.genreIds.push(genreId);
    } else {
      // Якщо жанр вже вибраний, видаліть його з масиву
      this.sortQuery.genreIds.splice(index, 1);
    }
    this.sortPublications(); // Виклик методу сортування після зміни жанрів
  }
  
  onPageChange(): void {
    // Ensure startPage is less than or equal to endPage
    if (this.sortQuery.startPage > this.sortQuery.endPage) {
      // Swap startPage and endPage if startPage is greater than endPage
      const temp = this.sortQuery.startPage;
      this.sortQuery.startPage = this.sortQuery.endPage;
      this.sortQuery.endPage = temp;
    }
    this.sortPublications();
  }
  onYearPublicationChange(): void {
    this.sortPublications();
  }
  // Add methods for handling startPage, endPage, and yearPublication changes if needed
}
