using System;
using System.Collections.Generic;
using System.Globalization;

namespace DocumentManager.Localization {
	// I was going to call this class I18n, but I thought Str would be
	// much simpler to type every time :)
	public class Str {
		#region Culture and Language
		private static readonly CultureInfo CulturePtBr = new CultureInfo("pt-BR");
		private static readonly CultureInfo CultureEn = CultureInfo.InvariantCulture;

		public const int LanguagePtBr = 0;
		public const int LanguageEn = 1;

		// After profiling the code below, I decided it would be better to go on this way.
		// The times shown are the sum of 10M executions.
		//
		// if (Str.CurrentLanguage == Str.LanguagePtBr) = ~110 ms
		// if (CultureInfo.CurrentUICulture.Name == "pt-BR") = ~375 ms
		// if (Thread.CurrentThread.CurrentUICulture.Name == "pt-BR") = ~640 ms
		//
		// Those results confirm the comment inside both
		// CultureInfo.CurrentCulture and CultureInfo.CurrentUICulture attributes
		// https://referencesource.microsoft.com/#mscorlib/system/globalization/cultureinfo.cs,712
		// https://referencesource.microsoft.com/#mscorlib/system/globalization/cultureinfo.cs,811
		// (The comment is quoted below)
		// In the case of CoreCLR, Thread.m_CurrentCulture and
		// Thread.m_CurrentUICulture are thread static so as not to let
		// CultureInfo objects leak across AppDomain boundaries. The
		// fact that these fields are thread static introduces overhead
		// in accessing them (through Thread.CurrentCulture). There is
		// also overhead in accessing Thread.CurrentThread. In this
		// case, we can avoid the overhead of Thread.CurrentThread
		// because these fields are thread static, and so do not
		// require a Thread instance to be accessed.
		[ThreadStatic]
		public static int CurrentLanguage;

		// No getter/setter to improve thread static performance!
		public static void SetCurrentLanguage(int languageId) {
			if (languageId == LanguageEn) {
				CurrentLanguage = LanguageEn;
				CultureInfo.CurrentCulture = CultureEn;
				CultureInfo.CurrentUICulture = CultureEn;
			} else {
				CurrentLanguage = LanguagePtBr;
				CultureInfo.CurrentCulture = CulturePtBr;
				CultureInfo.CurrentUICulture = CulturePtBr;
			}
		}
		#endregion

		#region Messages

		// Following the idea explained above, using this technique is faster
		// than having Resource Manager look up for the strings... which is done
		// using string comparison as described here:
		// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.2
		//
		// string x = Localizer["x"];

		public static string _o => CurrentLanguage == LanguageEn ? "the" : "o";
		public static string _a => CurrentLanguage == LanguageEn ? "the" : "a";
		public static string _os => CurrentLanguage == LanguageEn ? "the" : "os";
		public static string _as => CurrentLanguage == LanguageEn ? "the" : "as";
		public static string _O => CurrentLanguage == LanguageEn ? "The" : "O";
		public static string _A => CurrentLanguage == LanguageEn ? "The" : "A";
		public static string _Os => CurrentLanguage == LanguageEn ? "The" : "Os";
		public static string _As => CurrentLanguage == LanguageEn ? "The" : "As";
		public static string _um_a => CurrentLanguage == LanguageEn ? "a" : "um";
		public static string _uma_a => CurrentLanguage == LanguageEn ? "a" : "uma";
		public static string _um_an => CurrentLanguage == LanguageEn ? "an" : "um";
		public static string _uma_an => CurrentLanguage == LanguageEn ? "an" : "uma";

		public static string AppName => CurrentLanguage == LanguageEn ? "Repository of Academic Documents" : "Repositório de Documentos Acadêmicos";
		public static string Language => CurrentLanguage == LanguageEn ? "Language" : "Idioma";
		public static string Oops => "Oops...";
		public static string Portuguese => CurrentLanguage == LanguageEn ? "Portuguese" : "Português";
		public static string English => CurrentLanguage == LanguageEn ? "English" : "Inglês";
		public static string Yes => CurrentLanguage == LanguageEn ? "Yes" : "Sim";
		public static string No => CurrentLanguage == LanguageEn ? "No" : "Não";
		public static string Close => CurrentLanguage == LanguageEn ? "Close" : "Fechar";
		public static string Cancel => CurrentLanguage == LanguageEn ? "Cancel" : "Cancelar";
		public static string Create => CurrentLanguage == LanguageEn ? "Create" : "Criar";
		public static string Manage => CurrentLanguage == LanguageEn ? "Manage" : "Gerenciar";
		public static string Delete => CurrentLanguage == LanguageEn ? "Delete" : "Excluir";
		public static string Edit => CurrentLanguage == LanguageEn ? "Edit" : "Editar";
		public static string Error => CurrentLanguage == LanguageEn ? "Error" : "Erro";
		public static string SomethingWentWrong => CurrentLanguage == LanguageEn ? "Something went wrong! 😢" : "Algo saiu muito errado! 😢";
		public static string Dashboard => "Dashboard";
		public static string UserName => CurrentLanguage == LanguageEn ? "User Name" : "Login";
		public static string Password => CurrentLanguage == LanguageEn ? "Password" : "Senha";
		public static string Login => "Login";
		public static string Logout => "Logout";
		public static string NoPermission => CurrentLanguage == LanguageEn ? "No Permission" : "Sem Permissão";
		public static string NoAccessPermission => CurrentLanguage == LanguageEn ? "No access permission 😢" : "Sem permissão de acesso 😢";
		public static string MyProfile => CurrentLanguage == LanguageEn ? "My Profile" : "Meu Perfil";
		public static string EditProfile => CurrentLanguage == LanguageEn ? "Edit Profile" : "Editar Perfil";
		public static string FullName => CurrentLanguage == LanguageEn ? "Full Name" : "Nome Completo";
		public static string NewProfileImage => CurrentLanguage == LanguageEn ? "New Profile Image" : "Nova Foto de Perfil";
		public static string UpTo1MiB => CurrentLanguage == LanguageEn ? "Up to 1 MiB, preferably square" : "Até 1 MiB, de preferência quadrada";
		public static string InOrderToChangeCurrentPassword => CurrentLanguage == LanguageEn ? "In order to change your password, fill in all fields below" : "Para alterar a senha atual, preencha todos os campos abaixo";
		public static string CurrentPassword => CurrentLanguage == LanguageEn ? "Current Password" : "Senha Atual";
		public static string NewPassword => CurrentLanguage == LanguageEn ? "New Password" : "Nova Senha";
		public static string ConfirmNewPassword => CurrentLanguage == LanguageEn ? "Confirm New Password" : "Confirme Nova Senha";
		public static string SaveChanges => CurrentLanguage == LanguageEn ? "Save Changes" : "Salvar Alterações";
		public static string YourBrowserDoesNotSupportAdvancedFileHandling => CurrentLanguage == LanguageEn ? "Your browser does not support advanced file handling (please, use Firefox 13+ or Google Chrome 21+)" : "Seu browser não suporta tratamento avançado de arquivos (por favor, utilize o Firefox 13+ ou Google Chrome 21+)";
		public static string TheFileSizeMustBeAtMost => CurrentLanguage == LanguageEn ? "The file size must be at most {0} KiB" : "O tamanho do arquivo deve ser no máximo {0} KiB";
		public static string TheFileExtensionMustBe => CurrentLanguage == LanguageEn ? "The file extension must be {0}" : "A extensão do arquivo deve ser {0}";
		public static string ProfileSuccessfullyChanged => CurrentLanguage == LanguageEn ? "Profile successfully changed! 😄" : "Perfil alterado com sucesso! 😄";
		public static string AnErrorOccurredWhileReadingTheImage => CurrentLanguage == LanguageEn ? "An error occurred while reading the image: " : "Ocorreu um erro ao ler a foto: ";
		public static string CreateCourse => CurrentLanguage == LanguageEn ? "Create Course" : "Criar Curso";
		public static string EditCourse => CurrentLanguage == LanguageEn ? "Edit Course" : "Editar Curso";
		public static string ManageCourses => CurrentLanguage == LanguageEn ? "Manage Courses" : "Gerenciar Cursos";
		public static string DoYouReallyWantToDeleteTheCourse => CurrentLanguage == LanguageEn ? "Do you really want to delete the course?" : "Tem certeza que deseja excluir o curso?";
		public static string ThisOperationCannotBeUndone => CurrentLanguage == LanguageEn ? "This operation cannot be undone!" : "Essa operação NÃO pode ser desfeita!";
		public static string Course => CurrentLanguage == LanguageEn ? "Course" : "Curso";
		public static string course => CurrentLanguage == LanguageEn ? "course" : "curso";
		public static string theCourse => CurrentLanguage == LanguageEn ? "the course" : "o curso";
		public static string Courses => CurrentLanguage == LanguageEn ? "Courses" : "Cursos";
		public static string CoursesBasicInformation => CurrentLanguage == LanguageEn ? "Course's Basic Information" : "Informações Básicas do Curso";
		public static string Name => CurrentLanguage == LanguageEn ? "Name" : "Nome";
		public static string ShortName => CurrentLanguage == LanguageEn ? "Short Name" : "Apelido";
		public static string CourseSuccessfullyCreated => CurrentLanguage == LanguageEn ? "Course successfully created! 😄" : "Curso criado com sucesso! 😄";
		public static string CourseSuccessfullyChanged => CurrentLanguage == LanguageEn ? "Course successfully changed! 😄" : "Curso alterado com sucesso! 😄";
		public static string CourseNotFound => CurrentLanguage == LanguageEn ? "Course not found!" : "Curso não encontrado!";
		public static string InvalidName => CurrentLanguage == LanguageEn ? "Invalid name!" : "Nome inválido!";
		public static string InvalidShortName => CurrentLanguage == LanguageEn ? "Invalid short name!" : "Apelido inválido!";
		public static string NameTooLong => CurrentLanguage == LanguageEn ? "Name too long!" : "Nome muito longo!";
		public static string ShortNameTooLong => CurrentLanguage == LanguageEn ? "Short name too long!" : "Apelido muito longo!";
		public static string UserOrPasswordIsInvalid => CurrentLanguage == LanguageEn ? "User or password is invalid!" : "Usuário ou senha inválidos!";
		public static string AnErrorOccurredDuringTheLoginProcess => CurrentLanguage == LanguageEn ? "An error occurred during the login process 😢 - " : "Ocorreu um erro durante o processo de login 😢 - ";
		public static string PartitionTypes => CurrentLanguage == LanguageEn ? "Partition Types" : "Tipos de Partição";
		public static string DocumentTypes => CurrentLanguage == LanguageEn ? "Document Types" : "Tipos de Documento";
		public static string Documents => CurrentLanguage == LanguageEn ? "Documents" : "Documentos";
		public static string Profiles => CurrentLanguage == LanguageEn ? "Profiles" : "Perfis";
		public static string Users => CurrentLanguage == LanguageEn ? "Users" : "Usuários";
		public static string Creation => CurrentLanguage == LanguageEn ? "Creation" : "Criação";
		public static string Listing => CurrentLanguage == LanguageEn ? "Listing" : "Listagem";
		public static string Edition => CurrentLanguage == LanguageEn ? "Edition" : "Edição";
		public static string Deletion => CurrentLanguage == LanguageEn ? "Deletion" : "Exclusão";
		public static string InvalidPermission => CurrentLanguage == LanguageEn ? "Invalid permission!" : "Permissão inválida!";
		public static string EditingProfileNotAllowed => CurrentLanguage == LanguageEn ? "Editing the profile \"ADMINISTRATOR\" is not allowed!" : "Não é permitido editar o perfil \"ADMINISTRADOR\"!";
		public static string DeletingProfileNotAllowed => CurrentLanguage == LanguageEn ? "Deleting the profile \"ADMINISTRATOR\" is not allowed!" : "Não é permitido excluir o perfil \"ADMINISTRADOR\"!";
		public static string InvalidUserName => CurrentLanguage == LanguageEn ? "Invalid user name!" : "Login inválido!";
		public static string UserNameTooLong => CurrentLanguage == LanguageEn ? "User name too long!" : "Login muito longo!";
		public static string UserNameTooShort => CurrentLanguage == LanguageEn ? "User name too short!" : "Login muito curto!";
		public static string InvalidFullName => CurrentLanguage == LanguageEn ? "Invalid full name!" : "Nome completo inválido!";
		public static string FullNameTooLong => CurrentLanguage == LanguageEn ? "Full name too long!" : "Nome completo muito longo!";
		public static string FullNameTooShort => CurrentLanguage == LanguageEn ? "Full name too short!" : "Nome completo muito curto!";
		public static string ProfileNotFound => CurrentLanguage == LanguageEn ? "Profile not found!" : "Perfil não encontrado!";
		public static string NO_PROFILE => CurrentLanguage == LanguageEn ? "NO PROFILE" : "SEM PERFIL";
		public static string UsersCannotActivateThemselves => CurrentLanguage == LanguageEn ? "Users cannot activate themselves!" : "Um usuário não pode ativar a si próprio!";
		public static string UsersCannotDeactivateThemselves => CurrentLanguage == LanguageEn ? "Users cannot deactivate themselves!" : "Um usuário não pode desativar a si próprio!";
		public static string UsersCannotResetTheirPassword => CurrentLanguage == LanguageEn ? "Users cannot reset their own password!" : "Um usuário não pode redefinir sua própria senha!";
		public static string UsersCannotChangeTheirProfile => CurrentLanguage == LanguageEn ? "Users cannot change their own profile!" : "Um usuário não pode definir seu próprio perfil!";
		public static string InvalidImageFormat => CurrentLanguage == LanguageEn ? "Invalid image format!" : "Imagem com formato inválido!";
		public static string InvalidImageFile => CurrentLanguage == LanguageEn ? "Invalid image file!" : "Arquivo de imagem com formato inválido!";
		public static string ErrorSavingProfileImage => CurrentLanguage == LanguageEn ? "Error saving the profile image!" : "Falha na gravação da foto do perfil!";
		public static string InvalidPassword => CurrentLanguage == LanguageEn ? "Invalid password!" : "Password inválido!";
		public static string CurrentPasswordDoesNotMatch => CurrentLanguage == LanguageEn ? "Current password does not match \uD83D\uDE22" : "Senha atual não confere \uD83D\uDE22";
		public static string theDocumentType => CurrentLanguage == LanguageEn ? "the document type" : "o tipo de documento";
		public static string EditDocumentType => CurrentLanguage == LanguageEn ? "Edit Document Type" : "Editar Tipo de Documento";
		public static string documentType => CurrentLanguage == LanguageEn ? "document type" : "tipo de documento";
		public static string DocumentTypeNotFound => CurrentLanguage == LanguageEn ? "Document type not found!" : "Tipo de documento não encontrado!";
		public static string thePartitionType => CurrentLanguage == LanguageEn ? "the partition type" : "o tipo de partição";
		public static string EditPartitionType => CurrentLanguage == LanguageEn ? "Edit Partition Type" : "Editar Tipo de Partição";
		public static string partitionType => CurrentLanguage == LanguageEn ? "partition type" : "tipo de partição";
		public static string PartitionTypeNotFound => CurrentLanguage == LanguageEn ? "Partition type not found!" : "Tipo de partição não encontrado!";
		public static string theProfile => CurrentLanguage == LanguageEn ? "the profile" : "o perfil";
		public static string profile => CurrentLanguage == LanguageEn ? "profile" : "perfil";
		public static string user => CurrentLanguage == LanguageEn ? "user" : "usuário";
		public static string theUserName => CurrentLanguage == LanguageEn ? "the user name" : "o login";
		public static string Units => CurrentLanguage == LanguageEn ? "Units" : "Unidades";

		#endregion

		#region Localizable String
		public readonly string ValueEn, ValuePtBr;
		private readonly string CurrentValue;

		public Str(string valueEn, string valuePtBr) {
			ValueEn = valueEn;
			ValuePtBr = valuePtBr;
			CurrentValue = (CurrentLanguage == LanguageEn ? valueEn : valuePtBr);
		}

		public override string ToString() => CurrentValue;

		public override bool Equals(object obj) => CurrentValue.Equals(obj);

		public override int GetHashCode() => CurrentValue.GetHashCode();
		#endregion
	}
}
