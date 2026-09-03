namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// PersonMapper - Works AFTER data fecthed from Database Repository completed
    /// PersonMapper - Will NOT have impact on SQL query exectuted
    /// </summary>
    public static class PersonMapper
    {
        public static PersonDTO PersonToPersonDTO(Person person)
        {
            if (person == null) return null;

            return new PersonDTO
            {
                id = person.Id,
                firstName = person.firstName,
                lastName = person.lastName,
                rank = person.rank,
                category = person.category,
                dateOfBirth = person.dateOfBirth,
                isPlayCricket = person.isPlayCricket,
                dateCreated = person.dateCreated
            };
        }

        public static Person PersonDTOToPerson(PersonDTO dto)
        {
            if (dto == null) return null;

            return new Person
            {
                Id = dto.id,
                firstName = dto.firstName,
                lastName = dto.lastName,
                rank = dto.rank,
                category = dto.category,
                dateOfBirth = dto.dateOfBirth,
                isPlayCricket = dto.isPlayCricket,
                dateCreated = dto.dateCreated
            };
        }

        public static PersonCreateDTO PersonToPersonCreateDTO(Person person)
        {
            if (person == null) return null;

            return new PersonCreateDTO
            {
                firstName = person.firstName,
                lastName = person.lastName,
                rank = person.rank,
                category = person.category,
                dateOfBirth = person.dateOfBirth,
                isPlayCricket = person.isPlayCricket
            };
        }

        public static Person PersonCreateDTOToPerson(PersonCreateDTO dto)
        {
            if (dto == null) return null;

            return new Person
            {
                firstName = dto.firstName,
                lastName = dto.lastName,
                rank = dto.rank,
                category = dto.category,
                dateOfBirth = dto.dateOfBirth,
                isPlayCricket = dto.isPlayCricket
            };
        }
    }
}
