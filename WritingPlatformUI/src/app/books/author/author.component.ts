import { Component, OnInit } from '@angular/core';
import { UserService } from "../../_services/user.service";
import { Author } from "../../shared/models/author";

@Component({
  selector: 'app-author',
  templateUrl: './author.component.html',
  styleUrl: './author.component.css'
})
export class AuthorComponent {
  authors: Array<Author> = [];

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.userService.getAuthors().subscribe(x => this.authors = x);
    console.log(this.authors)
  }
}
