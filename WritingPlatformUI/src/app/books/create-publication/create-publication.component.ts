import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { Genre } from '../../shared/models/genre';
import { TokenStorageService } from '../../_services/token-storage.service';
import { ActivatedRoute } from '@angular/router';
import { PublicationCreate } from '../../shared/models/publication-create';

@Component({
  selector: 'app-create-publication',
  templateUrl: './create-publication.component.html',
  styleUrls: ['./create-publication.component.css']
})
export class CreatePublicationComponent implements OnInit {
  login: string = '';
  genres: Genre[] = [];
  publicationName: string = '';
  genreId: number = 0;
  filePath: File | null = null;
  titlePath: File | null = null;
  bookDescription: string = '';
  successMessage: string = '';

  constructor(
    private tokenStorageService: TokenStorageService,
    private route: ActivatedRoute,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.userService.getGenres().subscribe(genres => {
      this.genres = genres;
    });

    const user = this.tokenStorageService.getUser();
    if (user) {
      this.login = user.userName || null;
    }
  }

  onFileChange(event: any, field: string): void {
    const file = event.target.files[0];
    if (field === 'filePath') {
      this.filePath = file;
    } else if (field === 'titlePath') {
      this.titlePath = file;
    }
  }

  onSubmit(): void {
    if (this.filePath && this.titlePath) {
      const publication: PublicationCreate = {
        publicationName: this.publicationName,
        genreId: this.genreId,
        userName: this.login,
        filePath: this.filePath,
        titlePath: this.titlePath,
        bookDescription: this.bookDescription
      };
      this.userService.CreatePublication(publication).subscribe(() => {
        // Handle success: reset form fields and show success message
        this.resetForm();
        this.successMessage = 'Publication created successfully!';
      });
    }
  }

  resetForm(): void {
    this.publicationName = '';
    this.genreId = 0;
    this.filePath = null;
    this.titlePath = null;
    this.bookDescription = '';
  }
}
