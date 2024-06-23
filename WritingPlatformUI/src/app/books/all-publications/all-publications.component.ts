import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Publication } from '../../shared/models/publication';
import { UserService } from "../../_services/user.service";

@Component({
  selector: 'app-all-publications',
  templateUrl: './all-publications.component.html',
  styleUrls: ['./all-publications.component.css']
})
export class AllPublicationsComponent implements OnInit {
  publications: Publication[] = [];
  currentPage: number = 1;
  pageSize: number = 2;
  totalPages: number = 0;
  searchQuery: string = '';
  genreId: number | undefined;
  authorId: string | undefined;
  publicationName: string | undefined;

  constructor(private route: ActivatedRoute, private userService: UserService, private router: Router) { }

  ngOnInit(): void {
    this.genreId = this.route.snapshot.params['genreId'];
    this.authorId = this.route.snapshot.params['authorId'];
    this.publicationName = this.route.snapshot.params['publicationName'];

    this.fetchPublications();
  }

  fetchPublications(): void {
    let observable;

    if (this.genreId) {
      observable = this.userService.getPublicationsByGenre(this.genreId, this.currentPage, this.pageSize);
    } else if (this.authorId) {
      observable = this.userService.getPublicationsByAuthor(this.authorId, this.currentPage, this.pageSize);
    } else if (this.publicationName) {
      observable = this.userService.getPublicationsByName(this.publicationName, this.currentPage, this.pageSize);
    } else {
      observable = this.userService.getAllPublications(this.currentPage, this.pageSize);
    }

    observable.subscribe(response => {
      this.publications = response;
      if (response.length > 0) {
        this.totalPages = response[0].paginatorCount;
      }
    });
  }

  searchPublicationsByName(): void {
    this.currentPage = 1;
    this.genreId = undefined;
    this.authorId = undefined;
    this.publicationName = this.searchQuery;
    this.fetchPublications();
  }

  onPageChange(pageNumber: number): void {
    this.currentPage = pageNumber;
    this.fetchPublications();
  }

  getPaginatedPublications(): Publication[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = Math.min(startIndex + this.pageSize, this.publications.length);
    return this.publications.slice(startIndex, endIndex);
  }
}
