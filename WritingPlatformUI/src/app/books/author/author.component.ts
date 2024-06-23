import { Component, OnInit } from '@angular/core';
import { UserService } from "../../_services/user.service";
import { Author } from "../../shared/models/author";

@Component({
  selector: 'app-author',
  templateUrl: './author.component.html',
  styleUrls: ['./author.component.css']
})
export class AuthorComponent implements OnInit {
  authors: Author[] = [];
  currentPage: number = 1;
  pageSize: number = 5;
  totalPages: number = 0;

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.fetchAuthors();
  }

  fetchAuthors(): void {
    this.userService.getAuthors(this.currentPage, this.pageSize).subscribe(response => {
      this.authors = response;
      if (response.length > 0) {
        this.totalPages = response[0].paginatorCount;
      }
    });
  }

  onPageChange(pageNumber: number): void {
    this.currentPage = pageNumber;
    this.fetchAuthors();
  }
}
