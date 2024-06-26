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
    sortByItemId: 1 
  };
  sortByItems: SortByItem[] = []; 
  sortDirection: string = 'asc';
  currentPage: number = 1;
  pageSize: number = 3; 
  totalPages: number = 0;

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.loadGenres();
    this.loadSortByItems(); 
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
      this.totalPages = Math.ceil(publications.length / this.pageSize);
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
      // If genre is not already selected, add it to the array
      this.sortQuery.genreIds.push(genreId);
    } else {
      // If genre is already selected, remove it from the array
      this.sortQuery.genreIds.splice(index, 1);
    }
    this.sortPublications(); 
  }
  
  onPageChange(pageNumber: number): void {
    this.currentPage = pageNumber;
  }

  getPaginatedPublications(): Publication[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = Math.min(startIndex + this.pageSize, this.publications.length);
    return this.publications.slice(startIndex, endIndex);
  }

  onPageInputChange(): void {
    if (this.sortQuery.startPage > this.sortQuery.endPage) {
      const temp = this.sortQuery.startPage;
      this.sortQuery.startPage = this.sortQuery.endPage;
      this.sortQuery.endPage = temp;
    }
    this.sortPublications();
  }

  onYearPublicationChange(): void {
    this.sortPublications();
  }
}
