const BusinessEntityNameDisplay = {
    name: 'BusinessEntityNameDisplay',
    props: ['businessEntity'],
    template: `
    <span>
        {{
            (businessEntity.customerType === 'individual')
            ? (businessEntity.firstName ? businessEntity.firstName : '') + ' ' + (businessEntity.lastName ? businessEntity.lastName : '') + (businessEntity.companyName ? ' (' + businessEntity.companyName + ')' : '')
            : (businessEntity.companyName ? businessEntity.companyName : '') + (businessEntity.firstName || businessEntity.lastName ? ' (' + (businessEntity.firstName ? businessEntity.firstName : '') + ' ' + (businessEntity.lastName ? businessEntity.lastName : '') + ')' : '')
        }}
    </span>`
}