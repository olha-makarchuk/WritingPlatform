import { Component, OnInit } from '@angular/core';
import { UserService } from "../../_services/user.service";
import { Author } from "../../shared/models/author";

@Component({
  selector: 'app-author',
  templateUrl: './author.component.html',
  styleUrls: ['./author.component.css']
})
export class AuthorComponent implements OnInit {
  authors: Array<Author> = [];
  currentPage: number = 1;
  pageSize: number = 5; // Adjust page size as needed
  totalPages: number = 0;

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.fetchAuthors();
  }

  fetchAuthors(): void {
    this.userService.getAuthors().subscribe(authors => {
      this.authors = authors;
      this.totalPages = Math.ceil(authors.length / this.pageSize);
    });
  }

  onPageChange(pageNumber: number): void {
    this.currentPage = pageNumber;
  }

  getPaginatedAuthors(): Author[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = Math.min(startIndex + this.pageSize, this.authors.length);
    return this.authors.slice(startIndex, endIndex);
  }
}
