import { Component } from '@angular/core';
import { Genre } from '../../shared/models/genre';
import { UserService } from "../../_services/user.service";
import { Publication } from '../../shared/models/publication';

@Component({
  selector: 'app-genre',
  templateUrl: './genre.component.html',
  styleUrl: './genre.component.css',
})
export class GenreComponent {
  genres: Array<Genre> = [];
  publications: Array<Publication> = [];

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.userService.getGenres().subscribe(x => this.genres = x);
  }
}
