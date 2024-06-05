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
  publications: Array<Publication> = [];
  currentPage: number = 1;
  pageSize: number = 3; // Adjust this value to control items per page
  totalPages: number = 0;
  
  constructor(private route: ActivatedRoute, private userService: UserService, private router: Router) { }

  ngOnInit(): void {
    const genreId = this.route.snapshot.params['genreId'];
    const authorId = this.route.snapshot.params['authorId'];
    this.fetchPublications(genreId, authorId);
  }

   fetchPublications(genreId?: number, authorId?: string): void {
    let observable;
    if (genreId) {
      observable = this.userService.getPublicationsByGenre(genreId);
    } else if (authorId) {
      observable = this.userService.getPublicationsByAuthor(authorId);
    } else {
      observable = this.userService.getAllPublications();
    }

    observable.subscribe(publications => {
      this.publications = publications;
      this.totalPages = Math.ceil(publications.length / this.pageSize);
    });
  }

  onPageChange(pageNumber: number) {
    this.currentPage = pageNumber;
  }

  getPaginatedPublications(): Publication[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = Math.min(startIndex + this.pageSize, this.publications.length);
    return this.publications.slice(startIndex, endIndex);
  }
}
