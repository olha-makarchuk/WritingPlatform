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

  constructor(private route: ActivatedRoute, private userService: UserService, private router: Router) { }

  ngOnInit(): void {
    const genreId = this.route.snapshot.params['genreId'];
    const authorId = this.route.snapshot.params['authorId'];

    if (genreId) {
      this.userService.getPublicationsByGenre(genreId).subscribe(publications => {
        this.publications = publications;
      });
    }
    else if (authorId) {
      this.userService.getPublicationsByAuthor(authorId).subscribe(publications => {
        this.publications = publications;
      });
    }
    else {
      this.userService.getAllPublications().subscribe(publications => {
        this.publications = publications;
      });
    }
  }
}
