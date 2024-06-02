import { Component } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { Genre } from '../../shared/models/genre';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.css'
})
export class CatalogComponent {
  login: string = '';
  genres: Genre[] = [];

  constructor(
    private userService: UserService
  ) { }
}
