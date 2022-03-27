# Crossing-Guard

## Présentation du projet

Le projet a été pensé pour l’UE « Concours drones », dans laquelle il est demandé
de proposer une solution innovante basée sur un drone ou un essaim de drones, afin de
répondre à une problématique choisie.
Notre projet s’inscrit dans le cadre d’une zone urbaine, et plus particulièrement dans
une ville intelligente. 

Dans les zones urbaines, il est difficile d’assurer sans problème le partage de la route entre les usagers. Il y a régulièrement des accidents entre des voitures et des piétons, qui sont souvent liés à de mauvais comportements de la part des automobilistes. 
En particulier, les automobilistes arrivent parfois trop vite aux abords d’un passage piéton et ne voient que trop tard les piétons traverser, ce qui peut créer un accident. 

Dans certaines situations, il peut aussi arriver qu’un piéton soit masqué par divers obstacles au moment où il commence à traverser (comme par exemple des voitures garées sur le côté), et les automobilistes ont alors du mal à le voir, ce qui crée une situation dangereuse. 

Il est donc nécessaire de trouver des solutions pour sécuriser au maximum les passages piétons.
Il est possible d’utiliser des drones pour permettre à des piétons de traverser un passage piéton de manière sécurisée. Pour cela, à l’aide de bornes situées de chaque côté du passage, le piéton appuierait sur un bouton situé sur la borne pour « appeler » des gardes de traversée, autrement des drones disséminés dans la zone urbaine utilisant ce
système de traversée intelligente. 

Ces drones arriveraient donc au niveau de la route afin d’une part d’alerter les voitures de la traversée qui s’apprête à se faire grâce à des LEDs clignotantes bleues et rouges, et d’autre part dissuader les voitures à passer quand même, comme bien des conducteurs font lorsque le piéton est toujours sur la voie opposée. 
Cette dissuasion passe par le blocage des deux voies de chaque côté du passage piéton par des drones imposants et pouvant abîmer la voiture d’un usager refusant de s’arrêter.

Ainsi, un piéton peut s’engager sur la route de manière plus sereine car il sait que les
usagers de la route s’approchant sont conscients de sa traversée. Une fois cette dernière
achevée, les gardes repartent. 

Dans le cadre d’une ville intelligente, il nous faudrait faire
en sorte que les drones soient répartis dans toute la ville, au niveau de stations sur
lesquelles ils se rechargent. 
Les boutons pressés par les piétons seraient connectés, ce qui fait que lorsqu’un piéton appuie dessus, la ville intelligente pourrait affecter les drones disponibles (et disposant de suffisant d’autonomie pour assurer une mission classique) les
plus proches au passage piéton correspondant, et ce de manière complètement autonome et dynamiques. Il serait aussi possible d’affecter les drones à des missions récurrentes, comme part exemple les sorties d’école le matin et le soir.

## Architecture du projet

L’architecture de notre projet correspond à l’architecture classique d’un projet Unity.
En particulier, le dépot est composé de plusieurs dossiers différents :

– `Assets/` est le dossier comprenant la quasi-totalité du projet. C’est dans ce dossier
que se trouvent tous les scripts, les prefabs (contenant des prefabs de la ville, du person-
nage incarné lors de la simulation, des voitures ou encore des drones), les materials ou
encore le contenu de la scène correspondant au scénario.

– `Build/` contient tous les fichiers nécessaires à l’exécution du scénario via le fichier
exécutable au format `.exe`.

– `Logs/` est le dossier comprend toutes les traces de messages, de warnings ou d’erreurs apparus dans l’application.

– `UserSettings/` contient les préférences de l’utilisateur dans l’éditeur.

– `Packages/` contient une multitude d’éléments compressés du projet nécessaire au
lancement de la simulation dans l’éditeur.

– `ProjectSettings/` contient quant à lui de nombreux fichiers de configuration pour
le projet projet.


## Lancement de la simulation

La simulation a été réalisée sur Unity 2020.3.23f1, elle devrait donc a minima être accessible via l'éditeur Unity de cette version ou d'une version ultérieure.

Il est également possible sur Windows de lancer directement l'exécutable CrossingGard.exe trouvable au chemin `./Build/CrossingGard.exe`.


## Démonstration

Une mission considérée comme classique est présentée dans le fichier GIF ci-dessous :

![utilisation_classique](./Images/demo.gif)

Le piéton s'approche du passage piéton et aperçoit des voitures arriver de chaque côté.

Plutôt que d'attendre qu'elles arrivent pour – peut-être – lui céder le passage, le piéton appuie sur le bouton de la borne, ce qui appelle un essaim de drone appelés gardes. 
Ces derniers dissuadent les voitures de passer en force et elles s'arrêtent : le piéton peut traverser plus sereinement.

Une fois que le piéton a fini de traverser, les drones s'envolent et les voitures reprennent leur route.

## Commandes

Le piéton avance lorsqu'on appuie sur la touche `W` du clavier. Il est possible de le faire tourner sur lui-même en utilisant les flèches directionnelles du clavier.

L'appui sur le bouton d'une borne se fait en cliquant dessus.