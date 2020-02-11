using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Binairo
{
    internal enum Etat
    { Initial, Création, EnJeu };

    public partial class Form1 : Form
    {
        #region Champs globaux et propriétés

        // Nous déclarons des champs globaux et propriétés
        // hasChanged, etat, grille, N, historique, N01V, N01H et elapsed.

        #region HasChanged et hasChanged

        /// Champ et propriété pour suivre l'état de la grille.
        private bool HasChanged;

        private bool hasChanged
        {
            get { return HasChanged; }
            set // hasChanged = true;
            {
                HasChanged = value;
                tsChanged.Text = (HasChanged) ? "La grille a été modifiée."
                                             : "La grille est inchangée­";
            }
        }

        #endregion HasChanged et hasChanged

        #region État de l'application

        private Etat State;

        private Etat etat
        {
            get { return State; }
            set
            {
                State = value;
                switch (State)
                {
                    case Etat.Création:
                        tsState.Text = "Création d'une grille";
                        MenuAnnuler.Enabled = false;
                        break;

                    case Etat.Initial: tsState.Text = "État Initial"; MenuAnnuler.Enabled = false; break;
                    case Etat.EnJeu: tsState.Text = "Partie en cours"; break;
                }
            }
        }

        #endregion État de l'application

        #region Grille du jeu

        /// <summary> Case[,] grille;
        /// Nous avons besoin d'une grille de boutons pour représenter les cases.
        /// Par défaut, nous créons une grille 8x8. Au besoin, la grille sera
        /// réallouée à une autre dimension.
        /// </summary>
        private Case[,] grille;

        #endregion Grille du jeu

        #region N

        /// <summary> int N;
        /// Par défaut, la grille est de dimension 8x8.
        /// Donc, par défaut, N vaudra 8.
        /// Par contre, lorsqu'on lit une grille, on pourra avoir
        /// une grille de 10x10 ou 12x12.
        /// </summary>int N
        private int N;

        #endregion N

        #region Historique

        /// <summary> List<Contenu> historique;
        /// Afin de permettre l'annulation, on maintient une liste des cases
        ///    modifiées.
        /// Si on annule, on remplace le contenu d'une case par le dernier
        ///    Contenu de l'historique. Puis, on supprime cet objet de la liste.
        /// </summary>
        private List<Contenu> historique;

        #endregion Historique

        #region Annotations

        /// <summary> Les annotations
        /// On a deux tableaux d'annotations un pour les ligne et
        /// l'autre pour le collonnes.
        /// Les annotations seront placées lors de la création d'une grille.
        /// La classe Annotation fournira les outils pour la mise à jour
        /// des annotations.
        /// </summary>
        private Annotation[] annotationVertical, annotationHorizontal;

        #endregion Annotations

        #region Le temps écoulé

        /// <summary> int elapsed;
        /// Ce champ sert à cumuler le temps écoulé depuis le début de la
        /// résolution d'une grille.
        /// Le temps est affiché à chaque seconde dans tsTime en appelant
        /// la méthode ShowTime().
        /// </summary>
        private int Elapsed;

        private int elapsed
        {
            get { return Elapsed; }
            set
            {
                Elapsed = value;
                ShowTime();
            }
        }

        #endregion Le temps écoulé

        #endregion Champs globaux et propriétés

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region Notes et pseudocode

            //      S'occuper des champs globaux du formulaire
            //      hasChanged, etat, grille, N, historique, N01V, N01H et elapsed.
            //      Désactiver Cancel Enregistrer
            //      Initialiser le timer
            //      Initialiser la barre de status
            //      Afficher une grille par défaut

            #endregion Notes et pseudocode

            #region CODE

            hasChanged = false;
            MenuAnnuler.Enabled = false;
            elapsed = 0;
            grille = null;
            annotationVertical = annotationHorizontal = null;
            historique = new List<Contenu>();
            AfficherGrilleParDefaut();

            #endregion CODE
        }

        private void AfficherGrilleParDefaut()
        {
            string[] GrilleParDefaut =
            {
                "1     1 ",
                "  01 1  ",
                "  0     ",
                "1  1  1 ",
                "        ",
                "11    0 ",
                "1  1  0 ",
                "  01    "
            };
            // Créer la grille
            grille = new Case[8, 8];
            N = 8;
            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                {
                    grille[row, col] =
                        new Case(col * 42 + 2, row * 42 + 2, row, col, panel, false, Case_Click);
                    Case b = grille[row, col];
                    b.Parent = panel;   // placer le bouton sur la grille
                    b.Text = GrilleParDefaut[row][col].ToString();
                    b.originale = (b.Text == "1") || (b.Text == "0");
                }

            annotationHorizontal = new Annotation[N];
            annotationVertical = new Annotation[N];
            // #### Ajuster les annotations

            // Création et affichage des labels annotations
            for (int ctr = 0; ctr < N; ctr++)
            {
                annotationHorizontal[ctr] = new Annotation(ctr * 42 + 58, 40);
                Controls.Add(annotationHorizontal[ctr]);
                annotationVertical[ctr] = new Annotation(20, ctr * 42 + 75);
                Controls.Add(annotationVertical[ctr]);
            }

            // Parcourir la grille pour mettre à jours le nombre de zéro et de un des annotations
            for (int row = 0; row < N; row++)
            {
                for (int col = 0; col < N; col++)
                {
                    // TODO: Remettre à 0 les valeurs des zéros et des uns
                    switch (grille[row, col].Text)
                    {
                        case "0":
                            annotationHorizontal[col].NbrZero++;
                            annotationVertical[row].NbrZero++;
                            break;

                        case "1":
                            annotationHorizontal[col].NbrUn++;
                            annotationVertical[row].NbrUn++;
                            break;
                    }
                }
            }

            // Mise à jours des annotations
            for (int ctr = 0; ctr < N; ctr++)
            {
                annotationHorizontal[ctr].MAJ();
                annotationVertical[ctr].MAJ();
            }

            etat = Etat.Initial;
        }

        private void MenuQuitter_Click(object sender, EventArgs e)
        {
            // Vérifier si la grille a changé.
            // Si oui, proposer de sauvegarder la grille.
            // Fermer l'application

            #region CODE

            // Ce code est commenté car il fait apparaitre le MessageBox 2 fois en quittant étant donné qu'il est déja dans FormClosing()
            /*            if (hasChanged)
                            switch (MessageBox.Show("Enregistrer la grille", "La grille a changé",
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                            {
                                case DialogResult.Cancel: return;
                                case DialogResult.Yes: tsSave_Click(sender, e); break;
                                case DialogResult.No: break;
                            }
            */
            Close();

            #endregion CODE
        }

        private void Case_Click(object sender, EventArgs e)
        {
            #region Notes et pseudocode

            // Obtenir une référence sur la case cliquée.
            // Si la case est originale, on ne peut la changer.
            // Sauvegarder l'état actuel de la case (pour pouvoir annuler)
            //    Ici, il faut faire attention, si on clique la même case,
            //    on ne doit sauver qu'un historique.
            // Changer la valeur de la case ==> annotations aussi.
            // La grille est-elle complétée?
            //   Si oui, arrêter le temps, désactiver le menu annuler, afficher un message.
            //   Si non, activer le menu annuler, en jeu, activer le timer.

            #endregion Notes et pseudocode

            #region CODE

            // Obtenir une référence sur la case cliquée.
            Case c = sender as Case;

            // Si la case est originale, on ne peut la changer.
            if (c.originale) return;

            // Sauvegarder l'état actuel de la case (pour pouvoir annuler)
            //    Ici, il faut faire attention, si on clique la même case,
            //    on ne doit sauver qu'un historique.
            Case precedent = null;
            Contenu pred = null;
            if (historique.Count > 0)
            {
                pred = historique[historique.Count - 1];
                precedent = grille[pred.row, pred.col];
            }
            if ((precedent != c))
                historique.Add(new Contenu(c.row, c.col, c.Text[0], false));
            // Changer la valeur de la case ==> annotations aussi.
            switch (c.Text[0])
            {
                case ' ':
                    c.SetValue("0");
                    // zero++
                    annotationHorizontal[c.col].NbrZero++;
                    annotationHorizontal[c.col].MAJ();
                    annotationVertical[c.row].NbrZero++;
                    annotationVertical[c.row].MAJ();
                    break;

                case '0':
                    c.SetValue("1");
                    // zero--
                    // un++
                    annotationHorizontal[c.col].NbrZero--;
                    annotationHorizontal[c.col].NbrUn++;
                    annotationHorizontal[c.col].MAJ();
                    annotationVertical[c.row].NbrZero--;
                    annotationVertical[c.row].NbrUn++;
                    annotationVertical[c.row].MAJ();
                    break;

                case '1':
                    c.SetValue(" ");
                    // un--
                    annotationHorizontal[c.col].NbrUn--;
                    annotationHorizontal[c.col].MAJ();
                    annotationVertical[c.row].NbrUn--;
                    annotationVertical[c.row].MAJ();
                    break;
            }

            // La grille est-elle complétée?
            //   Si oui, arrêter le temps, désactiver le menu annuler,
            //      afficher un message.

            //   Si non, activer le menu annuler, en jeu, activer le timer.
            hasChanged = true;
            MenuAnnuler.Enabled = true;
            if (etat == Etat.Initial) etat = Etat.EnJeu;
            timer.Enabled = (etat == Etat.EnJeu);

            #endregion CODE
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // Incrémenter le timer
            // Mettre à jour l'affichage
            if (etat != Etat.EnJeu) return;
            elapsed++;
        }

        private void ShowTime()
        {
            // affiche le temps dans tsTime
            string m = (elapsed / 60).ToString();
            if (m.Length < 2) m = "0" + m;
            string s = (elapsed % 60).ToString();
            if (s.Length < 2) s = "0" + s;
            tsTime.Text = m + ":" + s;
        }

        private void MenuAnnuler_Click(object sender, EventArgs e)
        {
            #region Notes et pseudocode

            // Il faut restaurer le dernier Contenu de l'historique
            // Rappel: on ne peut arriver ici que si historique
            //         contient au moins un élément.
            // Mais, on va quand même vérifier.
            // Annuler la dernière action
            // Le bouton annuler est-il encore disponible?

            #endregion Notes et pseudocode

            #region CODE

            // L'écriture de cette méthode est laissée en TP.

            // ReVérifier si historique est vide et desactiver menu
            if (historique.Count == 0)
            {
                MenuAnnuler.Enabled = false;
                return;
            }

            // Remettre état précédent de la case
            grille[historique[historique.Count() - 1].row, historique[historique.Count() - 1].col].Text = historique[historique.Count() - 1].valeur.ToString();

            // *** Remmetre à jours les annotations ***
            // 1) Réinitialiser les annotations verticales et horizontales
            annotationHorizontal[historique[historique.Count() - 1].col].NbrZero = 0;
            annotationHorizontal[historique[historique.Count() - 1].col].NbrUn = 0;
            annotationVertical[historique[historique.Count() - 1].row].NbrZero = 0;
            annotationVertical[historique[historique.Count() - 1].row].NbrUn = 0;

            // 2) Parcourir la colonne, rangé par rangé, pour mettre à jours l'annotation verticale
            for (int row = 0; row < N; row++)
            {
                switch (grille[row, historique[historique.Count() - 1].col].Text)
                {
                    case "0":
                        annotationHorizontal[historique[historique.Count() - 1].col].NbrZero++;
                        break;

                    case "1":
                        annotationHorizontal[historique[historique.Count() - 1].col].NbrUn++;
                        break;
                }
            }

            // 3) Parcourir la rangé, colonne par colonne, pour mettre à jours l'annotation verticale
            for (int col = 0; col < N; col++)
            {
                switch (grille[historique[historique.Count() - 1].row, col].Text)
                {
                    case "0":
                        annotationVertical[historique[historique.Count() - 1].row].NbrZero++;
                        break;

                    case "1":
                        annotationVertical[historique[historique.Count() - 1].row].NbrUn++;
                        break;
                }
            }

            // 4) Mise à jours de l'affichage des annotations
            annotationHorizontal[historique[historique.Count() - 1].col].MAJ();
            annotationVertical[historique[historique.Count() - 1].row].MAJ();

            // Éffacer le dernier élément de l'historique
            historique.RemoveAt(historique.Count() - 1);

            // Si l'historique est vide, desactiver le menu Annuler
            if (historique.Count == 0)
            {
                MenuAnnuler.Enabled = false;
                return;
            }

            #endregion CODE
        }

        private void tsNew_Click(object sender, EventArgs e)
        {
            #region Notes et pseudocode

            //      S'occuper des champs globaux du formulaire
            //      Si la grille courante a changé, sauvegarder?
            //      Supprimer l'ancienne grille.
            //      Obtenir la dimension de la nouvelle grille
            //      Créer une grille NxN
            // NOTE : lorsque je crée ou supprime une grille, je
            //        dois m'occuper des annotations.

            #endregion Notes et pseudocode

            #region CODE

            if (hasChanged)
                switch (MessageBox.Show("Enregistrer la grille", "La grille a changé",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes: tsSave_Click(sender, e); break;
                    case DialogResult.No: break;
                }
            foreach (Case c in grille)
            {
                c.Visible = false; // effacer
                c.Parent = null;   // sera récupérée par le GC
            }
            // Pour obtenir la dimension de la nouvelle grille, il nous faudra un autre
            // formulaire.
            Form2 f = new Form2();
            f.ShowDialog();
            N = f.N;

            grille = new Case[N, N];
            for (int row = 0; row < N; row++)
                for (int col = 0; col < N; col++)
                {
                    grille[row, col] =
                        new Case(col * 42 + 2, row * 42 + 2, row, col, panel, false, Case_Click);
                    Case b = grille[row, col];
                    b.Parent = panel;   // placer le bouton sur la grille
                    b.SetValue(" ");
                }

            // Annotations ???
            foreach (Annotation annotation in annotationHorizontal)
            {
                annotation.Visible = false; // effacer
                annotation.Parent = null;   // sera récupérée par le GC
            }
            foreach (Annotation annotation in annotationVertical)
            {
                annotation.Visible = false; // effacer
                annotation.Parent = null;   // sera récupérée par le GC
            }

            // * Afficher et maj les annotations
            annotationHorizontal = new Annotation[N];
            annotationVertical = new Annotation[N];

            // Création et affichage des labels annotations
            for (int ctr = 0; ctr < N; ctr++)
            {
                annotationHorizontal[ctr] = new Annotation(ctr * 42 + 58, 40);
                Controls.Add(annotationHorizontal[ctr]);
                annotationVertical[ctr] = new Annotation(20, ctr * 42 + 75);
                Controls.Add(annotationVertical[ctr]);
            }

            // Réinitialisation des annotations
            for (int ctr = 0; ctr < N; ctr++)
            {
                annotationHorizontal[ctr].NbrUn = 0;
                annotationVertical[ctr].NbrZero = 0;
                annotationHorizontal[ctr].MAJ();
                annotationVertical[ctr].MAJ();
            }

            // Ajuster les dimensions du Panel
            panel.Width = panel.Height = N * 42 + 6;
            // Ajuster les dimensions de la fiche
            Width = panel.Width + 200;
            Height = panel.Height + 200;

            hasChanged = false;
            etat = Etat.Création;
            historique.Clear();
            elapsed = 0;

            #endregion CODE
        }

        private void tsLoad_Click(object sender, EventArgs e)
        {
            #region Notes et pseudocode

            //      Si la grille courante a changé, ...
            //      S'occuper des champs globaux du formulaire
            //      S'occuper des champs hérités et des composantes:
            //           Cancel, timer, etc.
            //      Déterminer le fichier à lire
            //      Détruire l'ancienne grille
            //      Lire la grille
            //      Supprimer l'historique
            //      Générer la nouvelle grille

            #endregion Notes et pseudocode

            #region CODE

            //      Si la grille courante a changé, ...
            if (hasChanged)
                switch (MessageBox.Show("Enregistrer la grille", "La grille a changé",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Cancel: return;
                    case DialogResult.Yes: tsSave_Click(sender, e); break;
                    case DialogResult.No: break;
                }

            //      Déterminer le fichier à lire
            OpenFileDialog d = new OpenFileDialog();
            d.Title = "Sélectionner le fichier à lire";
            if (d.ShowDialog() == DialogResult.Cancel) return;

            //      Lire la nouvelle grille
            Contenu[,] jeu = null; ;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream f = new FileStream(d.FileName, FileMode.Open);
                jeu = bf.Deserialize(f) as Contenu[,];
                f.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //      Détruire l'ancienne grille et annotations ?
            foreach (Case c in grille)
            {
                c.Parent = null;
                c.Visible = false;
            }
            foreach (Annotation annotation in annotationHorizontal)
            {
                annotation.Visible = false; // effacer
                annotation.Parent = null;   // sera récupérée par le GC
            }
            foreach (Annotation annotation in annotationVertical)
            {
                annotation.Visible = false; // effacer
                annotation.Parent = null;   // sera récupérée par le GC
            }

            //      Générer la nouvelle grille
            N = jeu.GetLength(0);

            grille = new Case[N, N];
            for (int row = 0; row < N; row++)
                for (int col = 0; col < N; col++)
                {
                    grille[row, col] =
                        new Case(col * 42 + 2, row * 42 + 2, row, col, panel, jeu[row, col].originale, Case_Click);
                    Case b = grille[row, col];
                    b.Parent = panel;   // placer le bouton sur la grille
                    b.SetValue(jeu[row, col].valeur.ToString());
                }

            // Générer les nouvelles annotations
            // Création et affichage des labels annotations
            for (int ctr = 0; ctr < N; ctr++)
            {
                annotationHorizontal[ctr] = new Annotation(ctr * 42 + 58, 40);
                Controls.Add(annotationHorizontal[ctr]);
                annotationVertical[ctr] = new Annotation(20, ctr * 42 + 75);
                Controls.Add(annotationVertical[ctr]);
            }

            // Parcourir la grille pour mettre à jours le nombre de zéro et de un des annotations
            for (int row = 0; row < N; row++)
            {
                for (int col = 0; col < N; col++)
                {
                    switch (grille[row, col].Text)
                    {
                        case "0":
                            annotationHorizontal[col].NbrZero++;
                            annotationVertical[row].NbrZero++;
                            break;

                        case "1":
                            annotationHorizontal[col].NbrUn++;
                            annotationVertical[row].NbrUn++;
                            break;
                    }
                }
            }

            // Mise à jours des annotations
            for (int ctr = 0; ctr < N; ctr++)
            {
                annotationHorizontal[ctr].MAJ();
                annotationVertical[ctr].MAJ();
            }

            //      S'occuper des champs globaux du formulaire
            historique.Clear();
            hasChanged = false;
            etat = Etat.EnJeu;
            //      S'occuper des champs hérités et des composantes:
            //           Cancel, timer, etc.
            // Ajuster les dimensions du Panel
            panel.Width = panel.Height = N * 42 + 6;
            // Ajuster les dimensions de la fiche
            Width = panel.Width + 200;
            Height = panel.Height + 200;
            elapsed = 0;
            timer.Enabled = false;

            #endregion CODE
        }

        private void tsSave_Click(object sender, EventArgs e)
        {
            #region Notes et pseudocode

            //      Identifier le fichier à créer
            //      Préparer l'objet à sérialiser
            //      Ouvrir le fichier et sérialiser
            //      S'assurer qu'on a traité tous les champs globaux.

            #endregion Notes et pseudocode

            #region CODE

            //      Identifier le fichier à créer
            SaveFileDialog d = new SaveFileDialog();
            d.Title = "Fichier où enregistrer la grille";
            if (d.ShowDialog() == DialogResult.Cancel) return;

            //      Préparer l'objet à sérialiser
            Contenu[,] jeu = new Contenu[N, N];
            for (int row = 0; row < N; row++)
                for (int col = 0; col < N; col++)
                {
                    jeu[row, col] =
                        new Contenu(row, col, grille[row, col].Text[0], grille[row, col].originale);
                    // Si on est en mode conception, les cases non vides sont toutes des cases originales
                    if ((etat == Etat.Création) && (grille[row, col].Text[0] != ' '))
                        jeu[row, col].originale = true;
                }
            //      Ouvrir le fichier et sérialiser
            BinaryFormatter bf = new BinaryFormatter();
            FileStream f = new FileStream(d.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            bf.Serialize(f, jeu);
            f.Close();

            //      S'assurer qu'on a traité tous les champs globaux.
            hasChanged = false;

            #endregion CODE
        }

        private void existetilUneSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region Notes et pseudocode

            // Ici, on dispose d'une méthode TrouveLaSolution()
            // qui retourne un booléen indiquant si elle a effectivement trouvé
            // une solution à la grille.
            // Cette méthode n'a besoin que des valeurs de la grille.
            // Il faut donc les lui passer ==> créer un tableau des valeurs.
            // Mais, avant, il faut s'assurer que la grille est cohérente.

            #endregion Notes et pseudocode

            #region CODE

            char[,] jeu = new char[N, N];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    jeu[i, j] = grille[i, j].Text[0];

            if (!Coherent(jeu))
            {
                MessageBox.Show(@"La grille n'est pas cohérente ==> pas de solution.");
                return;
            }

            // Effectuer la recherche active
            if (TrouveLaSolution(0, 0, jeu))
                MessageBox.Show("Une solution existe.");
            else MessageBox.Show("Aucune solution à cette grille.");

            #endregion CODE
        }

        /// <summary> bool Coherent(char[,] jeu)
        /// Cette méthode vérifie la cohérence de la grille.
        /// </summary>
        private bool Coherent(char[,] jeu)
        {
            #region PSEUDOCODE

            // Ici, il faut, entre autres, vérifier des choses comme:
            // pas plus de N/2 0 ou 1 sur une ligne ou une colonne.
            // pas plus de deux 1 ou deux 0 d'affilés.
            // Ça simplifierait la tâche si on avait un tableau de string
            // pour les lignes et un tableau de string pour les colonnes.
            // 1 -  Contruire les tableaux de chaines.
            // 2 -  Si on a "000" ou "111" dans une ligne ou une colonne, c'est incohérent!
            // 3 -  Si on a plus de N 0 ou plus de N 1 dans une ligne ou une
            //      colonne, c'est incohérent.

            #endregion PSEUDOCODE

            #region CODE

            #region Contruire les chaines pour effectuer les tests de cohérence

            string[] sr = new string[N]; // Chaines des lignes
            string[] sc = new string[N]; // Chaines des colonnes
            for (int r = 0; r < N; r++)
                for (int c = 0; c < N; c++)
                {
                    sr[r] += jeu[r, c];
                    sc[c] += jeu[r, c];
                }

            #endregion Contruire les chaines pour effectuer les tests de cohérence

            int n0, n1;

            #region Vérifier la cohérence des lignes

            foreach (string s in sr)
            {
                // A-t-on une ligne avec 000?
                if (s.IndexOf("000") >= 0) return false;
                // A-t-on une ligne avec 111?
                if (s.IndexOf("111") >= 0) return false;
                // A-t-on une ligne avec trop de 0 ou trop de 1?
                n0 = n1 = 0;
                foreach (char c in s)
                    if (c == '0') n0++;
                    else if (c == '1') n1++;
                if (n0 > N / 2) return false;
                if (n1 > N / 2) return false;
            }

            #endregion Vérifier la cohérence des lignes

            #region Vérifier la cohérence des colonnes

            foreach (string s in sc)
            {
                // A-t-on une colonne avec 000?
                if (s.IndexOf("000") >= 0) return false;
                // A-t-on une colonne avec 111?
                if (s.IndexOf("111") >= 0) return false;
                // A-t-on une colonne avec trop de 0 ou trop de 1?
                n0 = n1 = 0;
                foreach (char c in s)
                    if (c == '0') n0++;
                    else if (c == '1') n1++;
                if (n0 > N / 2) return false;
                if (n1 > N / 2) return false;
            }

            #endregion Vérifier la cohérence des colonnes

            #region Vérifier si des lignes/colonnes sont identiques

            for (int i = 0; i < N - 1; i++)
            {
                if (sr[i].Contains(" ")) continue;
                if (sc[i].Contains(" ")) continue;
                for (int j = i + 1; j < N; j++)
                {
                    if (sr[i] == sr[j]) return false;
                    if (sc[i] == sc[j]) return false;
                }
            }

            #endregion Vérifier si des lignes/colonnes sont identiques

            return true;

            #endregion CODE
        }

        private bool TrouveLaSolution(int row, int col, char[,] g)
        {
            #region PSEUDOCODE et EXPLICATIONS

            // L'idée ici est de supposer que jusqu'ici, la grille est cohérente et correspond à
            // une solution partielle. Remarquez que c'est obligatoirement le cas pour 0,0.
            // On place une valeur dans une case, on vérifie que c'est cohérent jusqu'ici et on se
            // rappelle récursivement sur la case suivante.
            // SI on a trouvé une valeur cohérente pour la dernière case, c'est qu'on a trouvé une solution.
            // C'est cette condition que l'on teste en premier.

            #endregion PSEUDOCODE et EXPLICATIONS

            #region CODE

            // 1 - Si row = N, c'est qu'on a une solution pour les N rangées.
            //     On a donc une solution pour toute la grille.
            //     On retourne donc true et c'est terminé.
            if (row == N)
                return true;

            // 2 - Si col = N, on doit passer à la prochaine rangée
            if (col == N)
            {
                // Trouver la solution pour la première case de la rangée suivante.
                return TrouveLaSolution(row + 1, 0, g);
            }

            // 3 - Si la case a déjà une valeur, on n'a pas à en trouver une.
            //     On passe donc à la case suivante.
            if (g[row, col] != ' ')
                return TrouveLaSolution(row, col + 1, g);

            // 4 - Si on arrive ici, c'est qu'on n'a pas encore de valeur pour cette case.
            //     On va donc essayer les valeurs 0 et 1. Si on trouve une solution
            //     avec une des ces valeurs, c'est OK. Sinon, on doit revenir en arrière.
            // 4.1 Voir si c'est possible avec un 0
            if (EstPossible('0', row, col, g))
            {
                g[row, col] = '0';
                if (TrouveLaSolution(row, col, g)) return true;
                else
                {
                    // Ici, on a déterminé que "0" ne mêne pas à une solution.
                    // On remet tout en place avant d'essayer avec un "1".
                    g[row, col] = ' ';
                }
            }
            // 4.2 Essayer avec un 1
            if (EstPossible('1', row, col, g))
            {
                g[row, col] = '1';
                if (TrouveLaSolution(row, col, g)) return true;
                else
                {
                    // Ici, on a déterminé que "1" ne mêne pas à une solution.
                    // On remet tout en place.
                    g[row, col] = ' ';
                }
            }
            // 5 - Si on arrive ici, c'est qu'on n'a pas trouvé de solution dans l'état
            //     actuel de la grille de jeu.
            //     Il faut donc revenir en arrière pour essayer d'autres valeurs
            //     pour les cases précédentes.
            return false;

            #endregion CODE
        }

        /// <summary> bool EstPossible
        /// Une valeur est possible (pour une case) si la grille demeure coherente
        /// avec cette valeur à cette case.
        /// </summary>
        private bool EstPossible(char c, int row, int col, char[,] jeu)
        {
            // Sauvegarder la valeur courante de cette case et la remplacer par la nouvelle
            char old = jeu[row, col];
            jeu[row, col] = c;
            // La grille est-elle encore cohérente?
            bool coherent = Coherent(jeu);
            // Restaurer l'ancienne valeur avant de retoourner le résultat
            jeu[row, col] = old;
            return coherent;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Vérifier si la grille a changé.
            // Si oui, proposer de sauvegarder la grille.

            #region CODE

            if (hasChanged)
                switch (MessageBox.Show("Enregistrer la grille", "La grille a changé",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Cancel: e.Cancel = true; break;
                    case DialogResult.Yes: tsSave_Click(sender, e); break;
                    case DialogResult.No: break;
                }

            #endregion CODE
        }
    }
}