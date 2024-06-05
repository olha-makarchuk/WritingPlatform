import { Component, OnInit } from '@angular/core';
import { Publication } from '../../shared/models/publication';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../_services/user.service';
import { TokenStorageService } from '../../_services/token-storage.service';

@Component({
  selector: 'app-my-publications',
  templateUrl: './my-publications.component.html',
  styleUrl: './my-publications.component.css'
})
export class MyPublicationsComponent implements OnInit {
  publications: Array<Publication> = [];
  isLoggedIn = false;
  userId: string = "";

  constructor(
    private tokenStorageService: TokenStorageService,
    private route: ActivatedRoute,
    private userService: UserService
  ) { }
  
  ngOnInit(): void {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    
    const user = this.tokenStorageService.getUser();
    if (user) {
      this.userId = user.userName || null;
    }

    this.userService.getPublicationsByAuthor(this.userId).subscribe(publications => {
      this.publications = publications;
    });
  }

  onDeletePublication(publicationId: number): void {
    this.userService.deletePublication(publicationId).subscribe(() => {
      this.publications = this.publications.filter(publication => publication.publicationId !== publicationId);
    });
  }
}



